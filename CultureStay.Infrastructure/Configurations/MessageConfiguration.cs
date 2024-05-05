using CultureStay.Domain;
using CultureStay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CultureStay.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.SenderName)
            .IsRequired();
        
        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(StringLength.MessageContent);

        builder.Property(m => m.SenderAvatarUrl)
            .HasMaxLength(StringLength.Url);
        

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}