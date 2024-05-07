using System.Linq.Expressions;
using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Specification;

namespace CultureStay.Domain.Repositories.Base;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> GetById(int id);
    
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? predicate = null);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity); 
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsAllExistAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    
    Task<TEntity?> FindOneAsync(ISpecification<TEntity> spec);
    Task<IEnumerable<TEntity>> FindListAsync(ISpecification<TEntity> spec);
    Task<int> CountAsync(ISpecification<TEntity> spec);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<double> AverageAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, double>> selector);
    Task<bool> AnyAsync(ISpecification<TEntity> spec);
    Task<bool> AnyAsync(int id);
        
    Task<(IEnumerable<TEntity>, int)> FindWithTotalCountAsync(ISpecification<TEntity> specification);
    Task<double> SumAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, double>> selector);
}