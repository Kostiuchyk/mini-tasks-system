using MiniTasksSystem.Api.Authorization;
using MiniTasksSystem.Api.Filters;
using MiniTasksSystem.Application.Projects;

namespace MiniTasksSystem.Api.Endpoints.Projects;

public static class ProjectEndpoints
{
    public static RouteGroupBuilder MapProjectEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IProjectService projectService) =>
        {
            var projects = await projectService.GetAll();

            return Results.Ok(projects.ToResponse());
        });

        group.MapGet("/{id}", async (string id, IProjectService projectService) =>
        {
            var project = await projectService.GetById(id);

            return Results.Ok(project.ToResponse());
        });

        group.MapPost("/", async (CreateProjectRequest request, IProjectService projectService) =>
        {
            var result = await projectService.Create(request.ToDto());

            return Results.Created($"/api/projects/{result.Id}", result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<CreateProjectRequest>>()
        .RequireAuthorization(PolicyNames.AdminOnly);

        group.MapPut("/{id}", async (string id, UpdateProjectRequest request, IProjectService projectService) =>
        {
            var result = await projectService.Update(id, request.ToDto());

            return Results.Ok(result.ToResponse());
        })
        .AddEndpointFilter<ValidationFilter<UpdateProjectRequest>>()
        .RequireAuthorization(PolicyNames.AdminOnly);

        group.MapDelete("/{id}", async (string id, IProjectService projectService) =>
        {
            await projectService.Delete(id);

            return Results.NoContent();
        })
        .RequireAuthorization(PolicyNames.AdminOnly);

        return group;
    }
}
