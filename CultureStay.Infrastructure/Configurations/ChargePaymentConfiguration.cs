using CultureStay.Domain;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class ChargePaymentConfiguration : IEntityTypeConfiguration<ChargePayment>
{
    public void Configure(EntityTypeBuilder<ChargePayment> builder)
    {
        builder.Property(cp => cp.PaymentCode)
            .IsRequired()
            .HasMaxLength(StringLength.PaymentCode);

        builder.Property(cp => cp.BookingPaymentCode)
            .IsRequired()
            .HasMaxLength(StringLength.PaymentCode);

        builder.Property(cp => cp.Amount)
            .IsRequired();

        builder.Property(cp => cp.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ChargePaymentStatus)Enum.Parse(typeof(ChargePaymentStatus), v));

        builder.HasOne(cp => cp.Host)
            .WithMany(h => h.ChargePayments)
            .HasForeignKey(cp => cp.HostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(cp => cp.CancellationTicket)
            .WithOne(ct => ct.ChargePayment)
            .HasForeignKey<ChargePayment>(cp => cp.CancellationTicketId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}