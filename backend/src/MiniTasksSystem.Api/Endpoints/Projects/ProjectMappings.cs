using MiniTasksSystem.Application.Projects;

namespace MiniTasksSystem.Api.Endpoints.Projects;

internal static class ProjectMappings
{
    internal static ProjectDto ToDto(this CreateProjectRequest request) =>
        new(string.Empty, request.Name, request.Description, default);

    internal static ProjectDto ToDto(this UpdateProjectRequest request) =>
        new(string.Empty, request.Name, request.Description, default);

    internal static ProjectResponse ToResponse(this ProjectDto dto) =>
        new(dto.Id, dto.Name, dto.Description, dto.CreatedAt);

    internal static List<ProjectResponse> ToResponse(this IEnumerable<ProjectDto> dtos) =>
        [.. dtos.Select(dto => dto.ToResponse())];
}
