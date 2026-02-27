using MiniTasksSystem.Application.Compliance;

namespace MiniTasksSystem.Api.Endpoints.Authentication;

public sealed record LoginRequest(
    [PersonalData] string Email,
    [SensitiveData] string Password);

public sealed record RegisterRequest(
    [PersonalData] string FullName,
    [PersonalData] string Email,
    [SensitiveData] string Password);

public sealed record RefreshRequest(string RefreshToken);
