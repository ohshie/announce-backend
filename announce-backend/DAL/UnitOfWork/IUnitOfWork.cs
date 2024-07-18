using Microsoft.EntityFrameworkCore;

namespace announce_backend.DAL.UnitOfWork;

public interface IUnitOfWork<out TContext> where TContext : DbContext
{
    TContext Context { get; }
    Task Save();
    void CreateTransaction();
    void Commit();
    void Rollback();
}