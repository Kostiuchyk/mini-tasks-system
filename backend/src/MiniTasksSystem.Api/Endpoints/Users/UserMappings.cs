using MiniTasksSystem.Application.Users;

namespace MiniTasksSystem.Api.Endpoints.Users;

internal static class UserMappings
{
    internal static UserResponse ToResponse(this UserDto dto) =>
        new(dto.Id, dto.FullName, dto.Email, dto.Role);

    internal static List<UserResponse> ToResponse(this IEnumerable<UserDto> dtos) =>
        dtos.Select(dto => dto.ToResponse()).ToList();
}
