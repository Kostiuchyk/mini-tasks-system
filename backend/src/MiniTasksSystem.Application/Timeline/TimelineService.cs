namespace MiniTasksSystem.Application.Timeline;

internal sealed class TimelineService(IAuditLogRepository auditLogRepository) : ITimelineService
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<List<AuditLogDto>> GetByTaskId(string taskId)
    {
        var logs = await _auditLogRepository.GetByTaskId(taskId);
        return logs.Select(l => new AuditLogDto(
            l.Id, l.TaskId, l.UserId, l.Action, l.OldValue, l.NewValue, l.CreatedAt)).ToList();
    }
}
