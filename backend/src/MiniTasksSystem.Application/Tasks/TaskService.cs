using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Common.Exceptions;
using MiniTasksSystem.Application.Comments;
using MiniTasksSystem.Application.Timeline;
using MiniTasksSystem.Domain.Tasks;

namespace MiniTasksSystem.Application.Tasks;

internal sealed class TaskService(ITaskItemRepository taskRepository, ICommentRepository commentRepository, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork) : ITaskService
{
    private readonly ITaskItemRepository _taskRepository = taskRepository;
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedResult<TaskItemDto>> GetTasks(TaskQueryParams query)
    {
        TaskItemStatus? status = null;
        if (!string.IsNullOrEmpty(query.Status) && Enum.TryParse<TaskItemStatus>(query.Status, out var parsed))
        {
            status = parsed;
        }

        var result = await _taskRepository.GetPaged(
            query.ProjectId, status, query.AssigneeId, query.Search, query.Page, query.PageSize);

        return new()
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<TaskItemDto> GetById(string id)
    {
        var task = await _taskRepository.GetById(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        return MapToDto(task);
    }

    public async Task<TaskItemDto> Create(TaskItemDto dto, string userId)
    {
        TaskItem task = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = Enum.Parse<TaskItemStatus>(dto.Status),
            Priority = Enum.Parse<TaskItemPriority>(dto.Priority),
            ProjectId = dto.ProjectId,
            AssigneeId = dto.AssigneeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            task.Number = await _taskRepository.GetNextNumberAsync(task.ProjectId);
            await _taskRepository.Create(task);

            await _auditLogRepository.Create(new()
            {
                TaskId = task.Id,
                UserId = userId,
                Action = "created",
                NewValue = task.Title,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(task);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<TaskItemDto> Update(string id, TaskItemDto dto, string userId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var task = await _taskRepository.GetById(id)
                ?? throw new NotFoundException(nameof(TaskItem), id);

            var oldStatus = task.Status.ToString();
            var oldPriority = task.Priority.ToString();
            var oldAssigneeId = task.AssigneeId;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = Enum.Parse<TaskItemStatus>(dto.Status);
            task.Priority = Enum.Parse<TaskItemPriority>(dto.Priority);
            task.AssigneeId = dto.AssigneeId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.Update(task);

            if (oldStatus != dto.Status)
            {
                await _auditLogRepository.Create(new()
                {
                    TaskId = task.Id,
                    UserId = userId,
                    Action = "status_changed",
                    OldValue = oldStatus,
                    NewValue = dto.Status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (oldPriority != dto.Priority)
            {
                await _auditLogRepository.Create(new()
                {
                    TaskId = task.Id,
                    UserId = userId,
                    Action = "priority_changed",
                    OldValue = oldPriority,
                    NewValue = dto.Priority,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (oldAssigneeId != dto.AssigneeId)
            {
                await _auditLogRepository.Create(new()
                {
                    TaskId = task.Id,
                    UserId = userId,
                    Action = "assignee_changed",
                    OldValue = oldAssigneeId,
                    NewValue = dto.AssigneeId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(task);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task Delete(string id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var task = await _taskRepository.GetById(id)
                ?? throw new NotFoundException(nameof(TaskItem), id);

            if (await _commentRepository.HasCommentsByTaskId(id))
            {
                throw new ValidationException([new ValidationError("Id", "Cannot delete a task that has comments. Remove all comments first.")]);
            }

            await _taskRepository.Delete(id);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static TaskItemDto MapToDto(TaskItem task) =>
        new(task.Id, task.Number, task.Title, task.Description, task.Status.ToString(),
            task.Priority.ToString(), task.ProjectId, task.AssigneeId,
            task.CreatedAt, task.UpdatedAt);
}
