using MiniTasksSystem.Api.Endpoints.Users;
using MiniTasksSystem.Application.Authentication;

namespace MiniTasksSystem.Api.Endpoints.Authentication;

internal static class AuthenticationMappings
{
    internal static AuthenticationResponse ToResponse(this AuthenticationResult result) =>
        new(result.AccessToken, result.RefreshToken, result.User.ToResponse());
}
