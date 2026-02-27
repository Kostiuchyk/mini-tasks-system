using MiniTasksSystem.Domain.Tasks;

namespace MiniTasksSystem.Application.Timeline;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetByTaskId(string taskId);
    Task Create(AuditLog auditLog);
}
