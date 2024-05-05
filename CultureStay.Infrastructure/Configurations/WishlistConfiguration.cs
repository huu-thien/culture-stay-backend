using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasOne(w => w.Guest)
            .WithMany(g => g.Wishlists)
            .HasForeignKey(w => w.GuestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.Property)
            .WithMany(p => p.Wishlists)
            .HasForeignKey(w => w.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}