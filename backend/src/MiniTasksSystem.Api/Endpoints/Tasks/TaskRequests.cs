namespace MiniTasksSystem.Api.Endpoints.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string Status,
    string Priority,
    string ProjectId,
    string? AssigneeId);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    string Status,
    string Priority,
    string? AssigneeId);
