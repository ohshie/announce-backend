using Microsoft.EntityFrameworkCore.Storage;

namespace announce_backend.DAL.UnitOfWork;

public class UnitOfWork<TContext>(ILogger<UnitOfWork<TContext>> logger, TContext context) 
    : IUnitOfWork<TContext>, IDisposable where TContext : Microsoft.EntityFrameworkCore.DbContext
{
    public TContext Context { get; private set; } = context;
    private bool _disposed;
    private IDbContextTransaction _objTran;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public async Task Save()
    {
        try
        {
            logger.LogInformation("Attempting to save changes to into database");
            await Context.SaveChangesAsync();
        }
        catch (Exception dbEx)
        {
            logger.LogError("Something wrong happened when attempting to save changes to Db. ");
        }
    }

    public void CreateTransaction()
    {
        _objTran = Context.Database.BeginTransaction();
    }

    public void Commit()
    {
        _objTran.Commit();
    }

    public void Rollback()
    {
        _objTran.Rollback();
        _objTran.Dispose();
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                Context.Dispose();
        _disposed = true;
    }
}