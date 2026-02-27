namespace MiniTasksSystem.Api.Endpoints.Projects;

public sealed record ProjectResponse(string Id, string Name, string? Description, DateTime CreatedAt);
