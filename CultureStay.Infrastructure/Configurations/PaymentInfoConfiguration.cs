using CultureStay.Domain;
using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
{
    public void Configure(EntityTypeBuilder<PaymentInfo> builder)
    {
        builder.Property(pi => pi.BankName)
            .IsRequired()
            .HasMaxLength(StringLength.Name);

        builder.Property(pi => pi.AccountNumber)
            .IsRequired()
            .HasMaxLength(StringLength.Name);
        
        builder.Property(pi => pi.AccountHolder)
            .IsRequired()
            .HasMaxLength(StringLength.Name);

        builder.HasOne(pi => pi.Host)
            .WithOne(pi => pi.PaymentInfo)
            .HasForeignKey<PaymentInfo>(pi => pi.HostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}