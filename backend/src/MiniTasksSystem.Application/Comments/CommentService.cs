using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Timeline;
using MiniTasksSystem.Domain.Tasks;

namespace MiniTasksSystem.Application.Comments;

internal sealed class CommentService(ICommentRepository commentRepository, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork) : ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<CommentDto>> GetByTaskId(string taskId)
    {
        var comments = await _commentRepository.GetByTaskId(taskId);
        return comments.Select(c => new CommentDto(c.Id, c.TaskId, c.AuthorId, c.Text, c.CreatedAt)).ToList();
    }

    public async Task<CommentDto> Create(CommentDto dto)
    {
        Comment comment = new()
        {
            TaskId = dto.TaskId,
            AuthorId = dto.AuthorId,
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _commentRepository.Create(comment);

            await _auditLogRepository.Create(new()
            {
                TaskId = comment.TaskId,
                UserId = comment.AuthorId,
                Action = "comment_added",
                NewValue = comment.Text,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CommitTransactionAsync();
            return new CommentDto(comment.Id, comment.TaskId, comment.AuthorId, comment.Text, comment.CreatedAt);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
