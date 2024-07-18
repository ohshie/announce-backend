using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace announce_backend.DAL.Repository;

public class UserRepository(IUnitOfWork<AnnounceDbContext.AnnounceDbContext> unitOfWork, ILogger<UserRepository> logger) 
    : GenericRepository<User, AnnounceDbContext.AnnounceDbContext>(logger, unitOfWork), IUserRepository
{
    public async Task<User?> GetByName(LoginModel user)
    {
        logger.LogInformation("Fetching {Username} from db", user.Username);
        var registeredUser = await unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

        return registeredUser;
    }
}