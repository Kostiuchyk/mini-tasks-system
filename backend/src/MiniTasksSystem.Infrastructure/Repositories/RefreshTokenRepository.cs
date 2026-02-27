using MongoDB.Driver;
using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Domain.Users;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : IRefreshTokenRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<RefreshToken?> GetByToken(string token)
    {
        if (Session is { } s)
        {
            return await _context.RefreshTokens.Find(s, t => t.Token == token).FirstOrDefaultAsync();
        }

        return await _context.RefreshTokens.Find(t => t.Token == token).FirstOrDefaultAsync();
    }

    public async Task Create(RefreshToken refreshToken)
    {
        if (Session is { } s)
        {
            await _context.RefreshTokens.InsertOneAsync(s, refreshToken);
            return;
        }

        await _context.RefreshTokens.InsertOneAsync(refreshToken);
    }

    public async Task Revoke(string token)
    {
        if (Session is { } s)
        {
            await _context.RefreshTokens.DeleteOneAsync(s, t => t.Token == token);
            return;
        }

        await _context.RefreshTokens.DeleteOneAsync(t => t.Token == token);
    }

    public async Task RevokeAllByUserId(string userId)
    {
        if (Session is { } s)
        {
            await _context.RefreshTokens.DeleteManyAsync(s, t => t.UserId == userId);
            return;
        }

        await _context.RefreshTokens.DeleteManyAsync(t => t.UserId == userId);
    }
}
