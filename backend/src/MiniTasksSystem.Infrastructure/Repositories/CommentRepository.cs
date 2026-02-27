using MongoDB.Driver;
using MiniTasksSystem.Application.Comments;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class CommentRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : ICommentRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<List<Comment>> GetByTaskId(string taskId)
    {
        if (Session is { } s)
        {
            return await _context.Comments.Find(s, c => c.TaskId == taskId).SortBy(c => c.CreatedAt).ToListAsync();
        }

        return await _context.Comments.Find(c => c.TaskId == taskId).SortBy(c => c.CreatedAt).ToListAsync();
    }

    public async Task<bool> HasCommentsByTaskId(string taskId)
    {
        if (Session is { } s)
        {
            return await _context.Comments.Find(s, c => c.TaskId == taskId).AnyAsync();
        }

        return await _context.Comments.Find(c => c.TaskId == taskId).AnyAsync();
    }

    public async Task Create(Comment comment)
    {
        if (Session is { } s)
        {
            await _context.Comments.InsertOneAsync(s, comment);
            return;
        }

        await _context.Comments.InsertOneAsync(comment);
    }
}
