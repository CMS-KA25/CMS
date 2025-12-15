using CMS.Domain.Auth.Entities;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Configurations.EMR
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Primary Key (also FK to User)
            builder.HasKey(p => p.PatientID);

            // One-to-One relationship with User
            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Patient>(p => p.PatientID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(p => p.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(p => p.Address)
                .HasMaxLength(500);

            builder.Property(p => p.BloodGroup)
                .HasMaxLength(10);

            builder.Property(p => p.Allergies)
                .HasMaxLength(1000);

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
    
    public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
    {
        public void Configure(EntityTypeBuilder<Observation> builder)
        {
            // Primary Key
            builder.HasKey(o => o.ObservationID);
            
            // Foreign Keys
            builder.HasOne<PatientEncounter>()
                .WithMany()
                .HasForeignKey(o => o.EncounterID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<User>() // Staff reference
                .WithMany()
                .HasForeignKey(o => o.StaffID)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Properties
            builder.Property(o => o.ObservationName)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(o => o.ReferenceRange)
                .HasMaxLength(100);
                
            builder.Property(o => o.Value)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(o => o.Unit)
                .HasMaxLength(50);
                
            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(o => o.IsDeleted)
                .HasDefaultValue(false);
                
            // Soft Delete Query Filter
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
    
    public class MedicalReportConfiguration : IEntityTypeConfiguration<MedicalReport>
    {
        public void Configure(EntityTypeBuilder<MedicalReport> builder)
        {
            // Primary Key
            builder.HasKey(mr => mr.ReportID);
            
            // Foreign Keys
            builder.HasOne<PatientEncounter>()
                .WithMany()
                .HasForeignKey(mr => mr.EncounterID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<User>() // UploadedBy user
                .WithMany()
                .HasForeignKey(mr => mr.UploadedBy)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Properties
            builder.Property(mr => mr.FileUrl)
                .HasMaxLength(500);
                
            builder.Property(mr => mr.Findings)
                .IsRequired()
                .HasMaxLength(2000);
                
            builder.Property(mr => mr.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(mr => mr.IsDeleted)
                .HasDefaultValue(false);
                
            // Soft Delete Query Filter
            builder.HasQueryFilter(mr => !mr.IsDeleted);
        }
    }
    
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            // Primary Key
            builder.HasKey(p => p.PrescriptionID);
            
            // Foreign Keys
            builder.HasOne<PatientEncounter>()
                .WithMany()
                .HasForeignKey(p => p.EncounterID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(p => p.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Properties
            builder.Property(p => p.MedicationName)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(p => p.Unit)
                .HasMaxLength(50);
                
            builder.Property(p => p.Duration)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(p => p.Notes)
                .IsRequired()
                .HasMaxLength(1000);
                
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
                
            // Soft Delete Query Filter
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}