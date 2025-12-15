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

namespace CMS.Data.Configurations.Appointments
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Primary Key
            builder.HasKey(a => a.AppointmentID);
            
            // Foreign Keys
            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<TimeSlot>()
                .WithMany()
                .HasForeignKey(a => a.SlotID)
                .OnDelete(DeleteBehavior.NoAction);
                
            builder.HasOne<User>() // CreatedBy user
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Properties
            builder.Property(a => a.ReasonForVisit)
                .IsRequired()
                .HasMaxLength(1000);
                
            builder.Property(a => a.GoogleCalendarEventID)
                .HasMaxLength(255);
                
            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(a => a.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(a => a.IsDeleted)
                .HasDefaultValue(false);
                
            // Soft Delete Query Filter
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
    
    public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
    {
        public void Configure(EntityTypeBuilder<TimeSlot> builder)
        {
            // Primary Key
            builder.HasKey(ts => ts.SlotID);
            
            // Foreign Key
            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(ts => ts.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Properties
            builder.Property(ts => ts.IsAvailable)
                .HasDefaultValue(true);
                
            builder.Property(ts => ts.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(ts => ts.IsDeleted)
                .HasDefaultValue(false);
                
            // Soft Delete Query Filter
            builder.HasQueryFilter(ts => !ts.IsDeleted);
        }
    }
}