using announce_backend.Models;

namespace announce_backend.DAL.Repository.IRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByName(LoginModel user);
}