using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);

    public Task BeginTransactionAsync(CancellationToken ct) => _context.Database.BeginTransactionAsync(ct);

    public Task CommitTransactionAsync(CancellationToken ct) => _context.Database.CommitTransactionAsync(ct);

    public Task RollbackAsync(CancellationToken ct) => _context.Database.RollbackTransactionAsync(ct);
}