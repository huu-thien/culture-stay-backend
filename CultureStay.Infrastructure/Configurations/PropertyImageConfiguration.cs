using CultureStay.Domain;
using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.Property(pi => pi.Url)
            .IsRequired()
            .HasMaxLength(StringLength.Url);
        
        builder.Property(pi => pi.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(pi => pi.Property)
            .WithMany(p => p.PropertyImages)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}