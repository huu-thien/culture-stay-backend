using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CultureStay.Domain;
using CultureStay.Domain.Entities;

namespace CultureStay.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Ignore(r => r.IsExpired);
        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(StringLength.Token);
        
        builder.HasIndex(r => r.Token).IsUnique();
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
