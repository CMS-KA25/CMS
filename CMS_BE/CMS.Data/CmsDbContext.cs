using CMS.Data.Configurations.Appointments;
using CMS.Data.Configurations.Auth;
using CMS.Data.Configurations.Billing;
using CMS.Data.Configurations.Clinic;
using CMS.Data.Configurations.EMR;
using CMS.Data.Configurations.Notifications;
using CMS.Domain.Appointments.Entities;
using CMS.Domain.Auth.Entities;
<<<<<<< HEAD
using CMS.Domain.Billing.Entities;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Entities;
using CMS.Domain.Notifications.Entities;
=======
using CMS.Domain.NotificationModels;
>>>>>>> feature/auth-api
using Microsoft.EntityFrameworkCore;

namespace CMS.Data
{
    public class CmsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<PatientEncounter> PatientEncounters { get; set; }
        public DbSet<PatientQueue> PatientQueues { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<MedicalReport> MedicalReports { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<BillTemplate> BillTemplates { get; set; }
        public DbSet<PatientBill> PatientBills { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<User_Sessions> UserSessions { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        // Notification entities
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationChannel> NotificationChannels { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<NotificationQueue> NotificationQueues { get; set; }

        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply feature-specific configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEncounterConfiguration());
            modelBuilder.ApplyConfiguration(new PatientQueueConfiguration());
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalReportConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionConfiguration());
            modelBuilder.ApplyConfiguration(new BillTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new PatientBillConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
            modelBuilder.ApplyConfiguration(new AuthConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new VerificationCodeConfiguration());
            modelBuilder.ApplyConfiguration(new InvitationConfiguration());
            // modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            // modelBuilder.ApplyConfiguration(new BillingConfiguration());
            // modelBuilder.ApplyConfiguration(new CalendarConfiguration());
            // modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            // modelBuilder.ApplyConfiguration(new EMRConfiguration());
            // Notification entity configurations (copied from NotificationContext)

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.RecipientEmail).HasMaxLength(100);
                entity.Property(e => e.RecipientPhone).HasMaxLength(20);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                var options = new System.Text.Json.JsonSerializerOptions();
                entity.Property(e => e.Metadata)
                    .HasConversion(
                        v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, options),
                        v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, options)
                    )
                    .Metadata.SetValueComparer(
                        new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, object>?>(
                            (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && 
                                System.Text.Json.JsonSerializer.Serialize(c1, options) == 
                                System.Text.Json.JsonSerializer.Serialize(c2, options)),
                            c => c == null ? 0 : System.Text.Json.JsonSerializer.Serialize(c, options).GetHashCode(),
                            c => c == null ? null : 
                                System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                                    System.Text.Json.JsonSerializer.Serialize(c, options), 
                                    options)
                        ));
                entity.HasIndex(e => e.RecipientId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.ScheduledFor);
            });

            // NotificationTemplate configuration
            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Body).IsRequired();
                entity.Property(e => e.Variables).HasMaxLength(2000);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.ChannelType);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
            });

            // NotificationChannel configuration
            modelBuilder.Entity<NotificationChannel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExternalId).HasMaxLength(100);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.DeliveryReceipt).HasMaxLength(1000);
                var options2 = new System.Text.Json.JsonSerializerOptions();
                entity.Property(e => e.ProviderMetadata)
                    .HasConversion(
                        v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, options2),
                        v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, options2)
                    )
                    .Metadata.SetValueComparer(
                        new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, object>?>(
                            (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && 
                                System.Text.Json.JsonSerializer.Serialize(c1, options2) == 
                                System.Text.Json.JsonSerializer.Serialize(c2, options2)),
                            c => c == null ? 0 : System.Text.Json.JsonSerializer.Serialize(c, options2).GetHashCode(),
                            c => c == null ? null : 
                                System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                                    System.Text.Json.JsonSerializer.Serialize(c, options2), 
                                    options2)
                        ));
                entity.HasIndex(e => e.NotificationId);
                entity.HasIndex(e => e.ChannelType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.SentAt);
            });

            // NotificationPreference configuration
            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomSettings).HasMaxLength(2000);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.ChannelType);
                entity.HasIndex(e => e.UserRole);
            });

            // NotificationQueue configuration
            modelBuilder.Entity<NotificationQueue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.ProcessingNode).HasMaxLength(100);
                entity.HasIndex(e => e.NotificationId);
                entity.HasIndex(e => e.ScheduledFor);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
            });

            // Relationships
            modelBuilder.Entity<Notification>()
                .HasMany(n => n.Channels)
                .WithOne(c => c.Notification)
                .HasForeignKey(c => c.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Template)
                .WithMany()
                .HasForeignKey(n => n.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NotificationQueue>()
                .HasOne(q => q.Notification)
                .WithMany()
                .HasForeignKey(q => q.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}