namespace MiniTasksSystem.Domain.Tasks;

public sealed class Comment
{
    public string Id { get; set; } = null!;
    public string TaskId { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
