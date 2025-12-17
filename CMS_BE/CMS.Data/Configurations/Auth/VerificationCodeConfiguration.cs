using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Data.Configurations.Auth
{
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).HasDefaultValueSql("NEWID()");
            builder.Property(v => v.Code).IsRequired().HasMaxLength(10);
            builder.Property(v => v.Purpose).IsRequired().HasMaxLength(50);
            builder.Property(v => v.ExpiresAt).IsRequired();
            builder.Property(v => v.IsUsed).HasDefaultValue(false);
            builder.Property(v => v.CreatedAt).IsRequired();
        }
    }
}
