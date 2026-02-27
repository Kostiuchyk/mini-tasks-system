namespace MiniTasksSystem.Domain.Projects;

public sealed class Project
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
