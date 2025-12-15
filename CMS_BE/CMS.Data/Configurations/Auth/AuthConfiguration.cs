using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Configurations.Auth
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(u => u.UserID);

            // Properties
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PhoneNumber)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .HasDefaultValue(UserRole.Patient);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }

    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            // Primary Key
            builder.HasKey(t => t.TokenID);

            // Foreign Key
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(t => t.GeneratedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            // Primary Key
            builder.HasKey(s => s.SessionID);

            // Foreign Key
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(s => s.SessionToken)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.DeviceInfo)
                .HasMaxLength(255);

            builder.Property(s => s.IPAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(s => s.UserAgent)
                .HasMaxLength(500);

            builder.Property(s => s.LogoutReason)
                .HasMaxLength(255);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Primary Key
            builder.HasKey(al => al.AuditID);

            // Foreign Key
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(al => al.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.ActionDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(al => al.ActionResult)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(al => al.CorrelationId)
                .HasMaxLength(50);

            builder.Property(al => al.IPAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(al => al.UserAgent)
                .HasMaxLength(500);

            builder.Property(al => al.ActionTimestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(al => al.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(al => !al.IsDeleted);
        }
    }
}