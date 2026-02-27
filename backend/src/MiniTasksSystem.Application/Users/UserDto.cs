using MiniTasksSystem.Application.Compliance;

namespace MiniTasksSystem.Application.Users;

public sealed record UserDto(
    string Id,
    string FullName,
    string Email,
    string Role);
