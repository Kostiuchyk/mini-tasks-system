using MongoDB.Driver;
using MiniTasksSystem.Application.Users;
using MiniTasksSystem.Domain.Users;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class UserRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : IUserRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<User?> GetById(string id)
    {
        if (Session is { } s)
        {
            return await _context.Users.Find(s, u => u.Id == id).FirstOrDefaultAsync();
        }

        return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmail(string email)
    {
        if (Session is { } s)
        {
            return await _context.Users.Find(s, u => u.Email == email).FirstOrDefaultAsync();
        }

        return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAll()
    {
        if (Session is { } s)
        {
            return await _context.Users.Find(s, _ => true).ToListAsync();
        }

        return await _context.Users.Find(_ => true).ToListAsync();
    }

    public async Task Create(User user)
    {
        if (Session is { } s)
        {
            await _context.Users.InsertOneAsync(s, user);
            return;
        }

        await _context.Users.InsertOneAsync(user);
    }
}
