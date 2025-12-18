using CMS.Domain.Appointments.Entities;
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

namespace CMS.Data.Configurations.Clinic
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // Primary Key (also FK to User)
            builder.HasKey(d => d.DoctorID);

            // One-to-One relationship with User
            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Doctor>(d => d.DoctorID)
                .OnDelete(DeleteBehavior.NoAction); // If User deleted, Doctor deleted

            // Properties
            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.Qualification)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.YearOfExperience)
                .IsRequired()
                .HasAnnotation("Range", "0,60"); // Will need custom validation

            builder.Property(d => d.StartTime)
                .HasDefaultValue(new TimeSpan(9, 0, 0));

            builder.Property(d => d.EndTime)
                .HasDefaultValue(new TimeSpan(18, 0, 0));

            builder.Property(d => d.SlotDuration)
                .HasDefaultValue(30);

            builder.Property(d => d.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }

    public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            // Primary Key
            builder.HasKey(l => l.LeaveID);

            // Foreign Key to User
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(l => l.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(l => l.Reason)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(l => l.IsFullDay)
                .HasDefaultValue(true);

            builder.Property(l => l.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }

    public class PatientEncounterConfiguration : IEntityTypeConfiguration<PatientEncounter>
    {
        public void Configure(EntityTypeBuilder<PatientEncounter> builder)
        {
            // Primary Key
            builder.HasKey(pe => pe.EncounterID);

            // Foreign Keys
            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(pe => pe.PatientID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(pe => pe.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<Appointment>()
                .WithMany()
                .HasForeignKey(pe => pe.AppointmentID)
                .OnDelete(DeleteBehavior.NoAction);

            // Self-referencing FK for Parent Encounter (optional)
            builder.HasOne<PatientEncounter>()
                .WithMany()
                .HasForeignKey(pe => pe.Parent_Encounter_ID)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Properties
            builder.Property(pe => pe.Description)
                .HasMaxLength(500);

            builder.Property(pe => pe.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(pe => !pe.IsDeleted);
        }
    }

    public class PatientQueueConfiguration : IEntityTypeConfiguration<PatientQueue>
    {
        public void Configure(EntityTypeBuilder<PatientQueue> builder)
        {
            // Primary Key
            builder.HasKey(pq => pq.QueueID);

            // Foreign Keys
            builder.HasOne<Appointment>()
                .WithMany()
                .HasForeignKey(pq => pq.AppointmentID)
                .OnDelete(DeleteBehavior.NoAction);

            // Properties
            builder.Property(pq => pq.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pq => pq.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pq => pq.IsDeleted)
                .HasDefaultValue(false);

            // Soft Delete Query Filter
            builder.HasQueryFilter(pq => !pq.IsDeleted);
        }
    }
}