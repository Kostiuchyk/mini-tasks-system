using MiniTasksSystem.Application.Timeline;

namespace MiniTasksSystem.Api.Endpoints.Timeline;

internal static class TimelineMappings
{
    internal static AuditLogResponse ToResponse(this AuditLogDto dto) =>
        new(dto.Id, dto.TaskId, dto.UserId, dto.Action, dto.OldValue, dto.NewValue, dto.CreatedAt);

    internal static List<AuditLogResponse> ToResponse(this IEnumerable<AuditLogDto> dtos) =>
        dtos.Select(dto => dto.ToResponse()).ToList();
}
