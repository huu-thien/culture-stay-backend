using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class VnpHistoryConfiguration : IEntityTypeConfiguration<VnpHistory>
{
    public void Configure(EntityTypeBuilder<VnpHistory> builder)
    {
        builder.HasOne(vnp=>vnp.BookingPayment)
            .WithMany(bp=>bp.VnpHistories)
            .HasForeignKey(vnp=>vnp.BookingPaymentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}