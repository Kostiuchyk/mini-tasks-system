namespace MiniTasksSystem.Application.Tasks;

public sealed record TaskItemDto(
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
