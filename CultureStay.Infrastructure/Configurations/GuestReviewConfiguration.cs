using CultureStay.Domain;
using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class GuestReviewConfiguration : IEntityTypeConfiguration<GuestReview>
{
    public void Configure(EntityTypeBuilder<GuestReview> builder)
    {
        builder.Property(gr => gr.Content)
            .IsRequired()
            .HasMaxLength(StringLength.ReviewContent);
        
        builder.HasOne(gr => gr.Guest)
            .WithMany(g => g.GuestReviews)
            .HasForeignKey(gr => gr.GuestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(gr => gr.Host)
            .WithMany(h => h.GuestReviews)
            .HasForeignKey(gr => gr.HostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}