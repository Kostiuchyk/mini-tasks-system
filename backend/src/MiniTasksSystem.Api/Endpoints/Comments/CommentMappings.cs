using MiniTasksSystem.Application.Comments;

namespace MiniTasksSystem.Api.Endpoints.Comments;

internal static class CommentMappings
{
    internal static CommentDto ToDto(this CreateCommentRequest request, string taskId, string authorId) =>
        new(string.Empty, taskId, authorId, request.Text, default);

    internal static CommentResponse ToResponse(this CommentDto dto) =>
        new(dto.Id, dto.TaskId, dto.AuthorId, dto.Text, dto.CreatedAt);

    internal static List<CommentResponse> ToResponse(this IEnumerable<CommentDto> dtos) =>
        dtos.Select(dto => dto.ToResponse()).ToList();
}
