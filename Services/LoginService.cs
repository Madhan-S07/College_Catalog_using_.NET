using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CollegeManagementApi.Models;
using CollegeManagementApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CollegeManagementApi.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration config;
        public LoginService(AppDbContext _appDbContext, IConfiguration _config)
        {
            appDbContext = _appDbContext;
            config = _config;       
        }
            public async Task<string?> PerformLogin(string username, string password)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();

            // Old SHA-256 password (64 characters): check it, then upgrade it to the new hash
            if (user.Password.Length == 64)
            {
                if (user.Password != ConvertToHash(password)) return null;

                user.Password = hasher.HashPassword(user, password);
                await appDbContext.SaveChangesAsync();
                return GenerateJWT_token(user.UserName, user.Role);
            }

            // New hash
            var result = hasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed) return null;

            return GenerateJWT_token(user.UserName, user.Role);
        }

        public async Task<bool> CreateAccount(string username, string password)
        {
            username = username.Trim();

            // username already taken
            var exists = await appDbContext.Users.AnyAsync(u => u.UserName == username);
            if (exists) return false;

            var user = new User { UserName = username, Password = string.Empty, Role = "User" };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, password);

            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        private string GenerateJWT_token(string usr_name , string role)

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
         { ClaimTypes.Role, role }, // Hook up your roles here

         { "Company", "Kaar infotech" }  // Custom user parameters

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