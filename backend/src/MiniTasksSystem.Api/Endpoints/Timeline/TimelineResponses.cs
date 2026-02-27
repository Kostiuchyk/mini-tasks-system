namespace MiniTasksSystem.Api.Endpoints.Timeline;

public sealed record AuditLogResponse(
    string Id,
    string TaskId,
    string UserId,
    string Action,
    string? OldValue,
    string? NewValue,
    DateTime CreatedAt);
