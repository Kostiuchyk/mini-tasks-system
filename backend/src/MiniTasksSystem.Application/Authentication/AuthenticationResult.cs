using MiniTasksSystem.Application.Users;

namespace MiniTasksSystem.Application.Authentication;

public sealed record AuthenticationResult(string AccessToken, string RefreshToken, UserDto User);
