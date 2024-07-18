using announce_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace announce_backend.DAL.AnnounceDbContext;

public class AnnounceDbContext(DbContextOptions<AnnounceDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public required DbSet<User> Users { get; set; }
    public required DbSet<VkToken> VkTokens { get; set; }
}