using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Application.Users;

public interface IUserRepository
{
    Task<User?> GetById(string id);
    Task<User?> GetByEmail(string email);
    Task<List<User>> GetAll();
    Task Create(User user);
}
