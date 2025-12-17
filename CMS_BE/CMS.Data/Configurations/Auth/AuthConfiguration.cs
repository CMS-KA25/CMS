using CMS.Domain.Auth.Entities;

using CMS.Domain.Auth.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Data.Configurations.Auth
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserID);

            builder.Property(u => u.UserID)
                .HasDefaultValueSql("NEWID()");

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserSessionConfiguration : IEntityTypeConfiguration<User_Sessions>
    {
        public void Configure(EntityTypeBuilder<User_Sessions> builder)
        {
            builder.HasKey(s => s.SessionID);

            builder.Property(s => s.SessionID)
                .HasDefaultValueSql("NEWID()");

            builder.Property(s => s.SessionToken)
                .IsRequired();

            builder.Property(s => s.DeviceInfo)
                .HasMaxLength(500);

            builder.Property(s => s.IPAddress)
                .HasMaxLength(50);

            builder.Property(s => s.UserAgent)
                .HasMaxLength(500);

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            // Explicitly map the relationship from the dependent side as well
            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserID);
            
            // Apply verification code configuration if any
            // (Added VerificationCodeConfiguration is registered in CmsDbContext)
        }
    }

    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasKey(t => t.TokenID);

            builder.Property(t => t.TokenID)
                .HasDefaultValueSql("NEWID()");

            builder.Property(t => t.AccessToken)
                .IsRequired();

            builder.HasIndex(t => t.UserID);
        }
    }
}
