using MongoDB.Driver;
using MiniTasksSystem.Application.Timeline;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class AuditLogRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : IAuditLogRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<List<AuditLog>> GetByTaskId(string taskId)
    {
        if (Session is { } s)
        {
            return await _context.AuditLogs.Find(s, a => a.TaskId == taskId).SortByDescending(a => a.CreatedAt).ToListAsync();
        }

        return await _context.AuditLogs.Find(a => a.TaskId == taskId).SortByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task Create(AuditLog auditLog)
    {
        if (Session is { } s)
        {
            await _context.AuditLogs.InsertOneAsync(s, auditLog);
            return;
        }

        await _context.AuditLogs.InsertOneAsync(auditLog);
    }
}
