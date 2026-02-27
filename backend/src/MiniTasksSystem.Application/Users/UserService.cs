using MiniTasksSystem.Application.Common.Exceptions;
using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Application.Users;

internal sealed class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<List<UserDto>> GetAll()
    {
        var users = await _userRepository.GetAll();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> GetById(string id)
    {
        var user = await _userRepository.GetById(id)
            ?? throw new NotFoundException(nameof(User), id);

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user) =>
        new(user.Id, user.FullName, user.Email, user.Role.ToString());
}
