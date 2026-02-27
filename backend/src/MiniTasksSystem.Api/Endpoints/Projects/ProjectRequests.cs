namespace MiniTasksSystem.Api.Endpoints.Projects;

public sealed record CreateProjectRequest(string Name, string? Description);

public sealed record UpdateProjectRequest(string Name, string? Description);
