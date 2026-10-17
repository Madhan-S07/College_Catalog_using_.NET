using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CollegeManagementApi.Models;
using CollegeManagementApi.Data;
using CollegeManagementApi.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CollegeManagementApi.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration config;
        private readonly ILogger<LoginService> _logger;

        public LoginService(AppDbContext _appDbContext, IConfiguration _config, ILogger<LoginService> logger)
        {
            appDbContext = _appDbContext;
            config = _config;
            _logger = logger;
        }

        public async Task<string?> PerformLogin(string username, string password)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user {User} not found", username);
                return null;
            }

            var hasher = new PasswordHasher<User>();

            // Old SHA-256 password (64 characters): check it, then upgrade it to the new hash
            if (user.Password.Length == 64)
            {
                if (user.Password != ConvertToHash(password))
                {
                    _logger.LogWarning("Login failed: wrong password for user {User}", username);
                    return null;
                }

                user.Password = hasher.HashPassword(user, password);
                await appDbContext.SaveChangesAsync();
                _logger.LogInformation("Password of user {User} upgraded to the new hash", username);
                _logger.LogInformation("User {User} logged in with role {Role}", user.UserName, user.Role);
                return GenerateJWT_token(user.UserName, user.Role);
            }

            // New hash
            var result = hasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed: wrong password for user {User}", username);
                return null;
            }

            _logger.LogInformation("User {User} logged in with role {Role}", user.UserName, user.Role);
            return GenerateJWT_token(user.UserName, user.Role);
        }

        public async Task<bool> CreateAccount(string username, string password)
        {
            username = username.Trim();

            // username already taken
            var exists = await appDbContext.Users.AnyAsync(u => u.UserName == username);
            if (exists)
            {
                _logger.LogWarning("Create account failed: username {User} already exists", username);
                return false;
            }

            var user = new User { UserName = username, Password = string.Empty, Role = "User" };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, password);

            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            _logger.LogInformation("Account created for user {User} with role User", username);
            return true;
        }

        private string GenerateJWT_token(string usr_name, string role)
        {
            var jwtSettings = config.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"]!;
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = double.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, usr_name },
                { ClaimTypes.Role, role },
                { "Company", "Kaar infotech" }
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }

        private string ConvertToHash(string rawPass)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawPass);
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}