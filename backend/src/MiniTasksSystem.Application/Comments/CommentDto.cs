namespace MiniTasksSystem.Application.Comments;

public sealed record CommentDto(string Id, string TaskId, string AuthorId, string Text, DateTime CreatedAt);
