using MiniTasksSystem.Api.Authorization;
using MiniTasksSystem.Api.Filters;
using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Application.Common.Exceptions;
using MiniTasksSystem.Application.Tasks;

namespace MiniTasksSystem.Api.Endpoints.Tasks;

public static class TaskEndpoints
{
    public static RouteGroupBuilder MapTaskEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            string? projectId,
            string? status,
            string? assigneeId,
            string? search,
            int? page,
            int? pageSize,
            ITaskService taskService) =>
        {
            int p = page ?? 1;
            int ps = pageSize ?? 10;

            List<ValidationError> errors = [];

            if (p < 1)
            {
                errors.Add(new("page", "Page must be at least 1."));
            }

            if (ps < 1 || ps > 100)
            {
                errors.Add(new("pageSize", "PageSize must be between 1 and 100."));
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }

            TaskQueryParams query = new(projectId, status, assigneeId, search, p, ps);

            var result = await taskService.GetTasks(query);

            return Results.Ok(result.ToResponse());
        });

        group.MapGet("/{id}", async (string id, ITaskService taskService) =>
        {
            var task = await taskService.GetById(id);

            return Results.Ok(task.ToResponse());
        });

        group.MapPost("/", async (CreateTaskRequest request, ITaskService taskService, IUserContext userContext) =>
        {
            var userId = userContext.UserId;

            var result = await taskService.Create(request.ToDto(), userId);

            return Results.Created($"/api/tasks/{result.Id}", result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<CreateTaskRequest>>()
        .RequireAuthorization(PolicyNames.AdminOnly);

        group.MapPut("/{id}", async (string id, UpdateTaskRequest request, ITaskService taskService, IUserContext userContext) =>
        {
            var userId = userContext.UserId;

            var result = await taskService.Update(id, request.ToDto(), userId);

            return Results.Ok(result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<UpdateTaskRequest>>()
        .RequireAuthorization(PolicyNames.AdminOnly);

        group.MapDelete("/{id}", async (string id, ITaskService taskService) =>
        {
            await taskService.Delete(id);

            return Results.NoContent();
        })
        .RequireAuthorization(PolicyNames.AdminOnly);

        return group;
    }
}
