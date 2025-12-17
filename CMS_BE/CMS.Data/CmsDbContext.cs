using Microsoft.EntityFrameworkCore;
using CMS.Domain;

namespace CMS.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.patient_id);

                entity.Property(e => e.patient_id)
                      .HasColumnName("patient_id")
                      .HasDefaultValueSql("NEWSEQUENTIALID()") 
                      .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.payment_id);

                entity.Property(e => e.payment_id)
                      .HasColumnName("payment_id")
                      .HasDefaultValueSql("NEWSEQUENTIALID()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Patient)
                      .WithMany()
                      .HasForeignKey(e => e.patient_id)
                      .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Apply feature-specific configurations
            // modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            // modelBuilder.ApplyConfiguration(new AuthConfiguration());
            // modelBuilder.ApplyConfiguration(new BillingConfiguration());
            // modelBuilder.ApplyConfiguration(new CalendarConfiguration());
            // modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            // modelBuilder.ApplyConfiguration(new EMRConfiguration());
            // modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }
}