namespace MiniTasksSystem.Application.Users;

public interface IUserService
{
    Task<List<UserDto>> GetAll();
    Task<UserDto> GetById(string id);
}
