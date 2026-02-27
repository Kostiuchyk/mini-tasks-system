using MiniTasksSystem.Api.Filters;
using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Application.Comments;

namespace MiniTasksSystem.Api.Endpoints.Comments;

public static class CommentEndpoints
{
    public static RouteGroupBuilder MapCommentEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{taskId}/comments", async (string taskId, ICommentService commentService) =>
        {
            var comments = await commentService.GetByTaskId(taskId);

            return Results.Ok(comments.ToResponse());
        });

        group.MapPost("/{taskId}/comments", async (
            string taskId,
            CreateCommentRequest request,
            ICommentService commentService,
            IUserContext userContext) =>
        {
            var userId = userContext.UserId;

            var result = await commentService.Create(request.ToDto(taskId, userId));

            return Results.Created($"/api/tasks/{taskId}/comments", result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<CreateCommentRequest>>();

        return group;
    }
}
