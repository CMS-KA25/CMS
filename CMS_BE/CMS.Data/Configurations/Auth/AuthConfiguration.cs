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

    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(s => s.SessionID);
            builder.Property(s => s.SessionToken).HasMaxLength(500).IsRequired();
            builder.Property(s => s.IPAddress).HasMaxLength(45).IsRequired();
            builder.Property(s => s.UserAgent).HasMaxLength(500);
            builder.Property(s => s.LogoutReason).HasMaxLength(100);
            
            builder.HasIndex(s => s.UserID);
            builder.HasIndex(s => s.SessionToken).IsUnique();
            builder.HasIndex(s => s.IsActive);
            
            builder.HasOne(s => s.User)
                   .WithMany(u => u.Sessions)
                   .HasForeignKey(s => s.UserID)
                   .OnDelete(DeleteBehavior.Cascade);
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

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.AuditID);
            
            builder.Property(a => a.Action).HasMaxLength(50).IsRequired();
            builder.Property(a => a.TableName).HasMaxLength(100).IsRequired();
            builder.Property(a => a.ErrorMessage).HasMaxLength(1000);
            builder.Property(a => a.CorrelationId).HasMaxLength(50);
            builder.Property(a => a.IPAddress).HasMaxLength(45);
            
            builder.HasIndex(a => a.UserID);
            builder.HasIndex(a => a.ActionTimestamp);
            builder.HasIndex(a => a.TableName);
        }
    }
}
