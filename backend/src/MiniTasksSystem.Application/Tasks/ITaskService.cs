using MiniTasksSystem.Application.Common;

namespace MiniTasksSystem.Application.Tasks;

public interface ITaskService
{
    Task<PagedResult<TaskItemDto>> GetTasks(TaskQueryParams query);
    Task<TaskItemDto> GetById(string id);
    Task<TaskItemDto> Create(TaskItemDto dto, string userId);
    Task<TaskItemDto> Update(string id, TaskItemDto dto, string userId);
    Task Delete(string id);
}
