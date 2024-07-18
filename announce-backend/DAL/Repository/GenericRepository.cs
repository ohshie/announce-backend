using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace announce_backend.DAL.Repository;

public class GenericRepository<TEntity, TContext> 
    : IRepository<TEntity> where TEntity : class 
    where TContext : DbContext
{
    protected readonly DbSet<TEntity> DbSet;
    protected ILogger<GenericRepository<TEntity, TContext>> Logger;
    
    protected GenericRepository(ILogger<GenericRepository<TEntity, TContext>> logger, IUnitOfWork<TContext> unitOfWork)
    {
        Logger = logger;
        DbSet = unitOfWork.Context.Set<TEntity>();
    }
    
    public async Task<TEntity> Get(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>?> GetAll()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<bool> Add(TEntity? entity)
    {
        if (entity is null) return false;
        
        await DbSet.AddAsync(entity);
        return true;
    }

    public async Task<bool> Update(TEntity? entity)
    {
        if (entity is null) return false;

        var entry = DbSet.Entry(entity);
        entry.State = EntityState.Modified;
        return true;
    }

    public async Task<bool> Remove(TEntity? entity)
    {
        if (entity is null) return false;

        DbSet.Remove(entity);
        
        return true;
    }
}