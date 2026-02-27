using MiniTasksSystem.Application.Timeline;

namespace MiniTasksSystem.Api.Endpoints.Timeline;

public static class TimelineEndpoints
{
    public static RouteGroupBuilder MapTimelineEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{taskId}", async (string taskId, ITimelineService timelineService) =>
        {
            var logs = await timelineService.GetByTaskId(taskId);

            return Results.Ok(logs.ToResponse());
        });

        return group;
    }
}
