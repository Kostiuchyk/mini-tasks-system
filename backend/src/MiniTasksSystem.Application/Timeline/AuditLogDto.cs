namespace MiniTasksSystem.Application.Timeline;

public sealed record AuditLogDto(
    string Id,
    string TaskId,
    string UserId,
    string Action,
    string? OldValue,
    string? NewValue,
    DateTime CreatedAt);
