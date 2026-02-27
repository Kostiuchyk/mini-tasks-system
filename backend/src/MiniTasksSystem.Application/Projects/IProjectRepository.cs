using MiniTasksSystem.Domain.Projects;

namespace MiniTasksSystem.Application.Projects;

public interface IProjectRepository
{
    Task<Project?> GetById(string id);
    Task<List<Project>> GetAll();
    Task Create(Project project);
    Task Update(Project project);
    Task Delete(string id);
}
