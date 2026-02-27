namespace MiniTasksSystem.Application.Tasks;

public sealed record TaskQueryParams(
    string? ProjectId = null,
    string? Status = null,
    string? AssigneeId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 10);
