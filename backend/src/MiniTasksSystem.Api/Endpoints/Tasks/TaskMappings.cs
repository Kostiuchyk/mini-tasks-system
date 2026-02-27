using MiniTasksSystem.Api.Common;
using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Tasks;

namespace MiniTasksSystem.Api.Endpoints.Tasks;

internal static class TaskMappings
{
    private const int DEFAULT_NUMBER = 0;

    internal static TaskItemDto ToDto(this CreateTaskRequest request) =>
        new(string.Empty,
            DEFAULT_NUMBER,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.ProjectId,
            request.AssigneeId,
            default,
            default);

    internal static TaskItemDto ToDto(this UpdateTaskRequest request) =>
        new(string.Empty,
            DEFAULT_NUMBER,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            string.Empty,
            request.AssigneeId,
            default,
            default);

    internal static TaskItemResponse ToResponse(this TaskItemDto dto) =>
        new(dto.Id,
            dto.Number,
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Priority,
            dto.ProjectId,
            dto.AssigneeId,
            dto.CreatedAt,
            dto.UpdatedAt);

    internal static PagedResponse<TaskItemResponse> ToResponse(this PagedResult<TaskItemDto> result) =>
        new([.. result.Items.Select(dto => dto.ToResponse())],
            result.TotalCount,
            result.Page,
            result.PageSize);
}
