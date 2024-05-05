using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class PropertyUtilityConfiguration : IEntityTypeConfiguration<PropertyUtility>
{
    public void Configure(EntityTypeBuilder<PropertyUtility> builder)
    {
        builder.HasOne(pu => pu.Property)
            .WithMany(p => p.PropertyUtilities)
            .HasForeignKey(pu => pu.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}