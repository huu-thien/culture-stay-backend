using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Enum;
using Message = CultureStay.Domain.Entities.Message;

namespace CultureStay.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    private readonly ICurrentUser _currentUser;
    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }
    
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    
    public DbSet<CancellationTicket> CancellationTickets { get; set; } = null!;
    
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<PropertyReview> PropertyReviews { get; set; } = null!;
    public DbSet<PropertyImage> PropertyImages { get; set; } = null!;
    public DbSet<PropertyUtility> PropertyAmenities { get; set; } = null!;
    
    public DbSet<Host> Hosts { get; set; } = null!;
    public DbSet<Guest> Guests { get; set; } = null!;
    public DbSet<HostReview> HostReviews { get; set; } = null!;
    public DbSet<GuestReview> GuestReviews { get; set; } = null!;
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<PaymentInfo> PaymentInfos { get; set; } = null!;
    public DbSet<BookingPayment> Payments { get; set; } = null!;
    public DbSet<ChargePayment> ChargePayments { get; set; } = null!;
    public DbSet<RefundPayment> RefundPayments { get; set; } = null!;
    public DbSet<HostPayment> HostPayments { get; set; } = null!;
    public DbSet<VnpHistory> VNPHistories { get; set; } = null!;
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.AddIdentitySeedData();
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Add soft delete query filter
        // https://www.thereformedprogrammer.net/ef-core-in-depth-soft-deleting-data-with-global-query-filters/
        var softDeleteEntityTypes = builder.Model
            .GetEntityTypes()
            .Where(e => e.ClrType.IsAssignableFrom(typeof(ISoftDelete)));
        
        foreach (var entityType in softDeleteEntityTypes)
            entityType.AddSoftDeleteQueryFilter();
        
        base.OnModelCreating(builder);
    }
    
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditableEntityProperties();
        SetSoftDeleteEntityProperties();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    private void SetAuditableEntityProperties()
    {
        var userId = int.TryParse(_currentUser.Id, out var id) ? id : 0;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.LastModifiedAt = DateTime.Now;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedOn = DateTime.Now;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.LastModifiedAt = DateTime.Now;
                    entry.Entity.IsDeleted = true;
                    break;
            }
        }
    }
    
    private void SetSoftDeleteEntityProperties()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State != EntityState.Deleted) continue;
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }
}

public static class SoftDeleteQueryExtension
{
    public static void AddSoftDeleteQueryFilter(
        this IMutableEntityType entityData)
    {
        var methodToCall = typeof(SoftDeleteQueryExtension)
            .GetMethod(nameof(GetSoftDeleteFilter))!
            .MakeGenericMethod(entityData.ClrType);
        
        var filter = methodToCall.Invoke(null, []);
        
        entityData.SetQueryFilter((LambdaExpression)filter!);
        entityData.AddIndex(entityData.FindProperty(nameof(ISoftDelete.IsDeleted))!);
    }
 
    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : ISoftDelete
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}