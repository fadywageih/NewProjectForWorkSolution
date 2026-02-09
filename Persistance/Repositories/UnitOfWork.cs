using Domain.Contracts;
using Domain.Entities;
using Persistance.Data;
using System.Collections.Concurrent;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new();
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            return (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeof(TEntity).Name
                , (_) => new GenericRepository<TEntity, TKey>(_dbContext));
        }
        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    }
}
