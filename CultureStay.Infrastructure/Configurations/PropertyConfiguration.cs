using CultureStay.Domain;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(StringLength.Title);

        builder.Property(p => p.Description)
            .HasMaxLength(StringLength.Description);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(StringLength.Address);

        builder.Property(p => p.Longitude)
            .HasPrecision(13, 10)
            .IsRequired();

        builder.Property(p => p.Latitude)
            .HasPrecision(13, 10)
            .IsRequired();

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(StringLength.City);
        
        builder.Property(p => p.Type)
            .HasConversion(v => v.ToString(), 
                v => (PropertyType)Enum.Parse(typeof(PropertyType), v));
        
        builder.HasOne(p => p.Host)
            .WithMany(h => h.Properties)
            .HasForeignKey(p => p.HostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}