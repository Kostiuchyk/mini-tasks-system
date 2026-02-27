namespace MiniTasksSystem.Application.Projects;

public sealed record ProjectDto(string Id, string Name, string? Description, DateTime CreatedAt);
