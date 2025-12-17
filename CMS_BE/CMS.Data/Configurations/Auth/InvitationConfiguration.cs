using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Data.Configurations.Auth
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.HasKey(i => i.InvitationId);
            builder.Property(i => i.InvitationId).HasDefaultValueSql("NEWID()");
            builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Role).IsRequired().HasMaxLength(50);
            builder.Property(i => i.Token).IsRequired().HasMaxLength(200);
            builder.Property(i => i.ExpiresAt).IsRequired();
            builder.Property(i => i.IsAccepted).HasDefaultValue(false);
            builder.Property(i => i.CreatedAt).IsRequired();
        }
    }
}
