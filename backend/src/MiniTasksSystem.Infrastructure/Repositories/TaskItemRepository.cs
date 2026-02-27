using MongoDB.Driver;
using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Tasks;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class TaskItemRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : ITaskItemRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<TaskItem?> GetById(string id)
    {
        if (Session is { } s)
        {
            return await _context.Tasks.Find(s, t => t.Id == id).FirstOrDefaultAsync();
        }

        return await _context.Tasks.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TaskItem>> GetPaged(
        string? projectId = null,
        TaskItemStatus? status = null,
        string? assigneeId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var filterBuilder = Builders<TaskItem>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(projectId))
        {
            filter &= filterBuilder.Eq(t => t.ProjectId, projectId);
        }

        if (status.HasValue)
        {
            filter &= filterBuilder.Eq(t => t.Status, status.Value);
        }

        if (!string.IsNullOrEmpty(assigneeId))
        {
            filter &= filterBuilder.Eq(t => t.AssigneeId, assigneeId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            filter &= filterBuilder.Regex(t => t.Title, new MongoDB.Bson.BsonRegularExpression(search, "i"));
        }

        var s = Session;

        var totalCount = s is not null
            ? await _context.Tasks.CountDocumentsAsync(s, filter)
            : await _context.Tasks.CountDocumentsAsync(filter);

        var find = s is not null
            ? _context.Tasks.Find(s, filter)
            : _context.Tasks.Find(filter);

        var items = await find
            .SortByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = (int)totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> HasTasksByProjectId(string projectId)
    {
        if (Session is { } s)
        {
            return await _context.Tasks.Find(s, t => t.ProjectId == projectId).AnyAsync();
        }

        return await _context.Tasks.Find(t => t.ProjectId == projectId).AnyAsync();
    }

    public async Task<int> GetNextNumberAsync(string projectId)
    {
        var filter = Builders<ProjectTaskCounter>.Filter.Eq(c => c.Id, projectId);
        var update = Builders<ProjectTaskCounter>.Update.Inc(c => c.Seq, 1);
        FindOneAndUpdateOptions<ProjectTaskCounter> options = new()
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        if (Session is { } s)
        {
            return (await _context.TaskCounters.FindOneAndUpdateAsync(s, filter, update, options)).Seq;
        }

        return (await _context.TaskCounters.FindOneAndUpdateAsync(filter, update, options)).Seq;
    }

    public async Task Create(TaskItem task)
    {
        if (Session is { } s)
        {
            await _context.Tasks.InsertOneAsync(s, task);
            return;
        }

        await _context.Tasks.InsertOneAsync(task);
    }

    public async Task Update(TaskItem task)
    {
        if (Session is { } s)
        {
            await _context.Tasks.ReplaceOneAsync(s, t => t.Id == task.Id, task);
            return;
        }

        await _context.Tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
    }

    public async Task Delete(string id)
    {
        if (Session is { } s)
        {
            await _context.Tasks.DeleteOneAsync(s, t => t.Id == id);
            return;
        }

        await _context.Tasks.DeleteOneAsync(t => t.Id == id);
    }
}
