using announce_backend.Models;

namespace announce_backend.DAL.Repository.IRepository;

public interface IVkTokenRepository : IRepository<VkToken>
{
    public Task<VkToken> GetDefault();
}