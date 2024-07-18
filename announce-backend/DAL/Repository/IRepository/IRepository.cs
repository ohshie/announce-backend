namespace announce_backend.DAL.Repository.IRepository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> Get(int id);
    Task<IEnumerable<TEntity>?> GetAll();
    Task<bool> Add(TEntity? entity);
    Task<bool> Update(TEntity? entity);
    Task<bool> Remove(TEntity? entity);
}