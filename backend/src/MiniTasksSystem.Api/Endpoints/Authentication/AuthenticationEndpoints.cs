using Microsoft.Extensions.Logging;
using MiniTasksSystem.Api.Filters;
using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Application.Compliance;

namespace MiniTasksSystem.Api.Endpoints.Authentication;

public static partial class AuthenticationEndpoints
{
    [LoggerMessage(Level = LogLevel.Information, Message = "New user login {Email}")]
    private static partial void LogUserLogin(
        ILogger logger,
        [PersonalData] string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "New user registration {FullName} {Email}")]
    private static partial void LogUserRegistration(
        ILogger logger,
        [PersonalData] string fullName,
        [PersonalData] string email,
        [SensitiveData] string password);

    public static RouteGroupBuilder MapAuthenticationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (LoginRequest request, IAuthenticationService authService, ILogger<LoginRequest> logger) =>
        {
            LogUserLogin(logger, request.Email);

            var result = await authService.Login(request.Email, request.Password);

            return Results.Ok(result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<LoginRequest>>()
        .AllowAnonymous();

        group.MapPost("/register", async (RegisterRequest request, IAuthenticationService authService, ILogger<RegisterRequest> logger) =>
        {
            LogUserRegistration(logger, request.FullName, request.Email, request.Password);

            var result = await authService.Register(request.FullName, request.Email, request.Password);

            return Results.Ok(result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<RegisterRequest>>()
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshRequest request, IAuthenticationService authService) =>
        {
            var result = await authService.Refresh(request.RefreshToken);

            return Results.Ok(result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<RefreshRequest>>()
        .AllowAnonymous();

        group.MapPost("/logout", async (RefreshRequest request, IAuthenticationService authService) =>
        {
            await authService.Logout(request.RefreshToken);

            return Results.NoContent();
        })
        .AllowAnonymous();

        return group;
    }
}
