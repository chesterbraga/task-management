namespace task_management.Services
{
    public interface ITokenService
    {
        string GenerateToken(string username, List<string> roles = null);
    }
}
