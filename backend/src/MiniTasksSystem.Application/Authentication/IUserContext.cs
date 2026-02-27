namespace MiniTasksSystem.Application.Authentication;

public interface IUserContext
{
    string UserId { get; }
    string Role { get; }
}
