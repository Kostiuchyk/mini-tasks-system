namespace MiniTasksSystem.Domain.Tasks;

public sealed class AuditLog
{
    public string Id { get; set; } = null!;
    public string TaskId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Action { get; set; } = null!;

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }
}
