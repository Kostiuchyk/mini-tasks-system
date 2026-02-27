using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Application.Authentication;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByToken(string token);
    Task Create(RefreshToken refreshToken);
    Task Revoke(string token);
    Task RevokeAllByUserId(string userId);
}
