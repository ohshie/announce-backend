using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace announce_backend.DAL.Repository;

public class VkTokenRepository(IUnitOfWork<AnnounceDbContext.AnnounceDbContext> unitOfWork, 
    ILogger<VkTokenRepository> logger) 
    : GenericRepository<VkToken, AnnounceDbContext.AnnounceDbContext>(logger, unitOfWork), 
        IVkTokenRepository
{
    public async Task<VkToken> GetDefault()
    {
        logger.LogInformation("Getting saved VkToken from db");
        var vkToken = await unitOfWork.Context.VkTokens.FirstOrDefaultAsync();

        if (vkToken is not null) return vkToken;
        
        logger.LogError("No VkToken is saved");
        return new VkToken();
    }
}