using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class HostConfiguration : IEntityTypeConfiguration<Host>
{
    public void Configure(EntityTypeBuilder<Host> builder)
    {
        builder.HasOne(h => h.User)
            .WithOne(u => u.Host)
            .HasForeignKey<Host>(h => h.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}