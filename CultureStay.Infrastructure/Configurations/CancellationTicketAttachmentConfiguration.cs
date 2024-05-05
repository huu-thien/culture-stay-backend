using CultureStay.Domain;
using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class CancellationTicketAttachmentConfiguration : IEntityTypeConfiguration<CancellationTicketAttachment>
{
    public void Configure(EntityTypeBuilder<CancellationTicketAttachment> builder)
    {
        builder.Property(cta => cta.Url)
            .IsRequired()
            .HasMaxLength(StringLength.Url);

        builder.HasOne(cta => cta.CancellationTicket)
            .WithMany(ct => ct.Attachments)
            .HasForeignKey(cta => cta.CancellationTicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}