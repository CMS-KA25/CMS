using CMS.Data.Configurations.Auth;
using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Data
{
    public class CmsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply feature-specific configurations
            // modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
             //modelBuilder.ApplyConfiguration(new AuthConfiguration());
            // modelBuilder.ApplyConfiguration(new BillingConfiguration());
            // modelBuilder.ApplyConfiguration(new CalendarConfiguration());
            // modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            // modelBuilder.ApplyConfiguration(new EMRConfiguration());
            // modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }
}