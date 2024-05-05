using CultureStay.Domain;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class CancellationTicketConfiguration : IEntityTypeConfiguration<CancellationTicket>
{
    public void Configure(EntityTypeBuilder<CancellationTicket> builder)
    {
        builder.Property(ct => ct.CancellationReason)
            .HasConversion(
                v => v.ToString(),
                v => (CancellationReason)Enum.Parse(typeof(CancellationReason), v));
        
        builder.Property(ct => ct.Status)
            .HasConversion(
                v => v.ToString(),
                v => (CancellationTicketStatus)Enum.Parse(typeof(CancellationTicketStatus), v));
        
        builder.Property(ct => ct.CancellationReasonNote)
            .HasMaxLength(StringLength.Description);

        builder.Property(ct => ct.ResolveNote)
            .HasMaxLength(StringLength.Description);
        
        builder.HasOne(ct => ct.Booking)
            .WithOne(b => b.CancellationTicket)
            .HasForeignKey<CancellationTicket>(ct => ct.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}