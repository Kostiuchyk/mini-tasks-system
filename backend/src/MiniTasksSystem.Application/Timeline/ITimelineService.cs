namespace MiniTasksSystem.Application.Timeline;

public interface ITimelineService
{
    Task<List<AuditLogDto>> GetByTaskId(string taskId);
}
