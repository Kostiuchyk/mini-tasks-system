namespace MiniTasksSystem.Domain.Tasks;

public sealed class TaskItem
{
    public string Id { get; set; } = null!;
    public int Number { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskItemPriority Priority { get; set; }

    public string ProjectId { get; set; } = null!;
    public string? AssigneeId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
