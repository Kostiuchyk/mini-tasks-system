using MiniTasksSystem.Application.Users;

namespace MiniTasksSystem.Api.Endpoints.Users;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAll();

            return Results.Ok(users.ToResponse());
        });

        group.MapGet("/{id}", async (string id, IUserService userService) =>
        {
            var user = await userService.GetById(id);

            return Results.Ok(user.ToResponse());
        });

        return group;
    }
}
