using Microsoft.Extensions.DependencyInjection;
using MiniTasksSystem.Application.Comments;
using MiniTasksSystem.Application.Projects;
using MiniTasksSystem.Application.Tasks;
using MiniTasksSystem.Application.Timeline;
using MiniTasksSystem.Application.Users;

namespace MiniTasksSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ITimelineService, TimelineService>();

        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
