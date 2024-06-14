using CultureStay.Domain;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.CheckInDate)
            .IsRequired();

        builder.Property(b => b.CheckOutDate)
            .IsRequired();
        builder.Property(b => b.TotalPrice)
            .IsRequired();
        builder.Property(b => b.PricePerNight)
            .IsRequired();
        
        builder.Property(b => b.SystemFee)
            .IsRequired();
        
        builder.Property(b => b.Status)
            .HasConversion(v => v.ToString(), 
                v => (BookingStatus)Enum.Parse(typeof(BookingStatus), v));
        builder.Property(b => b.Note)
            .HasMaxLength(StringLength.Description);
        
        builder.HasOne(b => b.Guest)
            .WithMany(g => g.Bookings)
            .HasForeignKey(b => b.GuestId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}