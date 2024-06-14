using CultureStay.Domain;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class RefundPaymentConfiguration : IEntityTypeConfiguration<RefundPayment>
{
    public void Configure(EntityTypeBuilder<RefundPayment> builder)
    {
        builder.Property(rp => rp.PaymentCode)
            .IsRequired()
            .HasMaxLength(StringLength.PaymentCode);
        
        builder.Property(rp => rp.BookingPaymentCode)
            .IsRequired()
            .HasMaxLength(StringLength.PaymentCode);
        
        builder.Property(rp => rp.Amount)
            .IsRequired();

        builder.Property(rp => rp.Status)
            .HasConversion(
                v => v.ToString(),
                v => (RefundPaymentStatus)Enum.Parse(typeof(RefundPaymentStatus), v));

        builder.HasOne(rp => rp.Guest)
            .WithMany(b => b.RefundPayments)
            .HasForeignKey(rp => rp.GuestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(rp => rp.CancellationTicket)
            .WithOne(ct => ct.RefundPayment)
            .HasForeignKey<RefundPayment>(rp => rp.CancellationTicketId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}