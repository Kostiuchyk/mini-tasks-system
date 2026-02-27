using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Common.Exceptions;
using MiniTasksSystem.Application.Tasks;
using MiniTasksSystem.Domain.Projects;

namespace MiniTasksSystem.Application.Projects;

internal sealed class ProjectService(IProjectRepository projectRepository, ITaskItemRepository taskItemRepository, IUnitOfWork unitOfWork) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ITaskItemRepository _taskItemRepository = taskItemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<ProjectDto>> GetAll()
    {
        var projects = await _projectRepository.GetAll();
        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto> GetById(string id)
    {
        var project = await _projectRepository.GetById(id)
            ?? throw new NotFoundException(nameof(Project), id);

        return MapToDto(project);
    }

    public async Task<ProjectDto> Create(ProjectDto dto)
    {
        Project project = new()
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.Create(project);
        return MapToDto(project);
    }

    public async Task<ProjectDto> Update(string id, ProjectDto dto)
    {
        var project = await _projectRepository.GetById(id)
            ?? throw new NotFoundException(nameof(Project), id);

        project.Name = dto.Name;
        project.Description = dto.Description;

        await _projectRepository.Update(project);
        return MapToDto(project);
    }

    public async Task Delete(string id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var project = await _projectRepository.GetById(id)
                ?? throw new NotFoundException(nameof(Project), id);

            if (await _taskItemRepository.HasTasksByProjectId(id))
            {
                throw new ValidationException([new ValidationError("Id", "Cannot delete a project that has tasks. Remove all tasks first.")]);
            }

            await _projectRepository.Delete(id);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static ProjectDto MapToDto(Project project) =>
        new(project.Id, project.Name, project.Description, project.CreatedAt);
}
