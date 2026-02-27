namespace MiniTasksSystem.Api.Endpoints.Tasks;

public sealed record TaskItemResponse(
    string Id,
    int Number,
    string Title,
    string? Description,
    string Status,
    string Priority,
    string ProjectId,
    string? AssigneeId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
