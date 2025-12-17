using CMS.Data.Configurations.Appointments;
using CMS.Data.Configurations.Auth;
using CMS.Data.Configurations.Billing;
using CMS.Data.Configurations.Clinic;
using CMS.Data.Configurations.EMR;
using CMS.Data.Configurations.Notifications;
using CMS.Domain.Appointments.Entities;
using CMS.Domain.Auth.Entities;
using CMS.Domain.Billing.Entities;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Entities;
using CMS.Domain.Notifications.Entities;
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
        }
    }
}