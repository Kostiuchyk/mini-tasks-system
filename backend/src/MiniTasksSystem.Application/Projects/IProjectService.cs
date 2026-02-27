namespace MiniTasksSystem.Application.Projects;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAll();
    Task<ProjectDto> GetById(string id);
    Task<ProjectDto> Create(ProjectDto dto);
    Task<ProjectDto> Update(string id, ProjectDto dto);
    Task Delete(string id);
}
