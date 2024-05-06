using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CultureStay.Domain;
using CultureStay.Domain.Entities;

namespace CultureStay.Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(StringLength.Name);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(StringLength.Email);
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(StringLength.Phone);
        builder.Property(u => u.Address)
            .HasMaxLength(StringLength.Address);
        builder.Property(u => u.City)
            .HasMaxLength(StringLength.City);
        builder.Property(u => u.State)
            .HasMaxLength(StringLength.State);
        builder.Property(u => u.Country)
            .HasMaxLength(StringLength.Country);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(StringLength.Url);
        builder.Property(u => u.Introduction)
            .HasMaxLength(StringLength.Description);

        // builder.HasData(
        //     new User
        //     {
        //         Id = 1,
        //         UserName = "admin",
        //         NormalizedUserName = "ADMIN",
        //         Email = "example@gmail.com",
        //         NormalizedEmail = "EXAMPLE@GMAIL.COM",
        //         EmailConfirmed = true,
        //         PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Admin@123"),
        //         SecurityStamp = Guid.NewGuid().ToString(),
        //         FullName = "Admin",
        //     });
    }
}

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<IdentityRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
    {
        // builder.HasData(
        //     new IdentityRole<int>
        //     {
        //         Id = 1,
        //         Name = "admin",
        //         NormalizedName = "ADMIN"
        //     },
        //     new IdentityRole<int>
        //     {
        //         Id = 2,
        //         Name = "user",
        //         NormalizedName = "USER"
        //     }
        // );
    }
}

public class ApplicationUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        // builder.HasData(
        //     new IdentityUserRole<int>
        //     {
        //         RoleId = 1,
        //         UserId = 1
        //     });
    }
}
