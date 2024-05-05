using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasOne(g => g.User)
            .WithOne(u => u.Guest)
            .HasForeignKey<Guest>(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}