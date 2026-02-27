using MiniTasksSystem.Domain.Tasks;

namespace MiniTasksSystem.Application.Comments;

public interface ICommentRepository
{
    Task<List<Comment>> GetByTaskId(string taskId);
    Task<bool> HasCommentsByTaskId(string taskId);
    Task Create(Comment comment);
}
