using MiniTasksSystem.Api.Endpoints.Users;

namespace MiniTasksSystem.Api.Endpoints.Authentication;

public sealed record AuthenticationResponse(string AccessToken, string RefreshToken, UserResponse User);
