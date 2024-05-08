using System.Linq.Expressions;
using CultureStay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Repositories.Base;
using CultureStay.Domain.Specification;

namespace CultureStay.Infrastructure.Repositories.Base;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    private DbSet<TEntity>? _dbSet;
    protected DbSet<TEntity> DbSet => _dbSet ??= _dbContext.Set<TEntity>();
    
    private readonly AppDbContext _dbContext;
    
    public RepositoryBase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return DbSet.AsQueryable().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public IQueryable<TEntity> GetById(int id) => DbSet.Where(e => e.Id == id);

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return DbSet.Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
    {
        return DbSet.Where(predicate).ToListAsync(ct);
    }

    public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? predicate = null)
        => predicate is null ? DbSet.AsQueryable() : DbSet.Where(predicate);
        

    public void Add(TEntity entity) => DbSet.Add(entity);
    
    public void Update(TEntity entity) => DbSet.Update(entity);
    
    public void Delete(TEntity entity) => DbSet.Remove(entity);
    
    public void DeleteRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
    
    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(e => e.Id == id, cancellationToken);

    public Task<bool> IsAllExistAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        => DbSet.AllAsync(e => ids.Contains(e.Id), cancellationToken);

    public async Task<TEntity?> FindOneAsync(ISpecification<TEntity> spec) 
        => await GetQuery<TEntity>.From(DbSet, spec).FirstOrDefaultAsync();

    public async Task<IEnumerable<TEntity>> FindListAsync(ISpecification<TEntity> spec)
        => await GetQuery<TEntity>.From(DbSet, spec).ToListAsync();

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
        => await GetQuery<TEntity>.From(DbSet, spec).CountAsync();

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        => await DbSet.CountAsync(predicate);

    public async Task<double> AverageAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, double>> selector)
        => await GetQuery<TEntity>.From(DbSet, spec).AverageAsync(selector);

    public async Task<bool> AnyAsync(ISpecification<TEntity> spec)
        => await GetQuery<TEntity>.From(DbSet, spec).AnyAsync();

    public async Task<bool> AnyAsync(int id)
        => await DbSet.AnyAsync(e => e.Id == id);

    public async Task<(IEnumerable<TEntity>, int)> FindWithTotalCountAsync(ISpecification<TEntity> specification)
    {
        var query = GetQuery<TEntity>.From(DbSet, specification);
        var count = await query.CountAsync();
        var data = await query.Skip(specification.Skip).Take(specification.Take).ToListAsync();
        return (data, count);
    }

    public async Task<double> SumAsync(ISpecification<TEntity> spec, Expression<Func<TEntity, double>> selector)
        => await GetQuery<TEntity>.From(DbSet, spec).SumAsync(selector);
}
