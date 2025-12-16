using CMS.Domain.Auth.Entities;
using CMS.Domain.Billing.Entities;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Configurations.Billing
{
    public class BillTemplateConfiguration : IEntityTypeConfiguration<BillTemplate>
    {
        public void Configure(EntityTypeBuilder<BillTemplate> builder)
        {
            // Primary Key
            builder.HasKey(bt => bt.TemplateID);

            // Properties
            builder.Property(bt => bt.TemplateName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(bt => bt.ServiceDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(bt => bt.IsActive)
                .HasDefaultValue(true);

            builder.Property(bt => bt.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(bt => bt.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(bt => bt.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(bt => !bt.IsDeleted);
        }
    }

    public class PatientBillConfiguration : IEntityTypeConfiguration<PatientBill>
    {
        public void Configure(EntityTypeBuilder<PatientBill> builder)
        {
            // Primary Key
            builder.HasKey(pb => pb.BillID);

            // Foreign Keys
            builder.HasOne<PatientEncounter>()
                .WithMany()
                .HasForeignKey(pb => pb.EncounterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<BillTemplate>()
                .WithMany()
                .HasForeignKey(pb => pb.TemplateID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<User>() // CreatedBy user
                .WithMany()
                .HasForeignKey(pb => pb.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(pb => pb.Description)
                .HasMaxLength(500);

            builder.Property(pb => pb.DiscountAmount)
                .HasDefaultValue(0);

            builder.Property(pb => pb.BillDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pb => pb.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pb => pb.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(pb => !pb.IsDeleted);
        }
    }

    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            // Primary Key
            builder.HasKey(i => i.InvoiceID);

            // Foreign Keys
            builder.HasOne<PatientBill>()
                .WithMany()
                .HasForeignKey(i => i.BillID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties

            builder.Property(i => i.InvoiceDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.PaymentDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}