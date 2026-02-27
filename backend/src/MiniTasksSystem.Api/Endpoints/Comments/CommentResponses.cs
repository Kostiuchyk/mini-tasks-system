namespace MiniTasksSystem.Api.Endpoints.Comments;

public sealed record CommentResponse(string Id, string TaskId, string AuthorId, string Text, DateTime CreatedAt);
