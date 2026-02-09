namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        Task<TEntity?> GetByIdAsync(Tkey id);
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
