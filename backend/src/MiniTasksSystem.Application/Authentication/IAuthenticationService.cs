namespace MiniTasksSystem.Application.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResult> Login(string email, string password);
    Task<AuthenticationResult> Register(string fullName, string email, string password);
    Task<AuthenticationResult> Refresh(string refreshToken);
    Task Logout(string refreshToken);
}
