using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Domain.Tasks;

namespace MiniTasksSystem.Application.Tasks;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetById(string id);
    Task<PagedResult<TaskItem>> GetPaged(
        string? projectId = null,
        TaskItemStatus? status = null,
        string? assigneeId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10);
    Task<bool> HasTasksByProjectId(string projectId);
    Task<int> GetNextNumberAsync(string projectId);
    Task Create(TaskItem task);
    Task Update(TaskItem task);
    Task Delete(string id);
}
