namespace CollegeManagementApi.Services
{
    public interface ILoginService
    {
       Task<string?> PerformLogin(string username, string password);
       Task<bool> CreateAccount(string username, string password);
    }
}