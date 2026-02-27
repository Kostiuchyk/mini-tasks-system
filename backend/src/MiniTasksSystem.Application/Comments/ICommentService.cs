namespace MiniTasksSystem.Application.Comments;

public interface ICommentService
{
    Task<List<CommentDto>> GetByTaskId(string taskId);
    Task<CommentDto> Create(CommentDto dto);
}
