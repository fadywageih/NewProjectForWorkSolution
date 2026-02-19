namespace Persistance.Repositories
{
    public class GenericRepository<TEntity, Tkey> : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);
        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false) =>
            asNoTracking ? await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync()
            : await _dbContext.Set<TEntity>().ToListAsync();
        public async Task<TEntity?> GetByIdAsync(Tkey id) => await _dbContext.Set<TEntity>().FindAsync(id);

    }
}
