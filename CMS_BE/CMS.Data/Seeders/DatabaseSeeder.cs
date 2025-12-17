using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CMS.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAdminUserAsync(CmsDbContext context, IConfiguration configuration)
        {
            // Check if admin user already exists
            var adminExists = await context.Users
                .AnyAsync(u => u.Role == RoleType.Admin);

            if (adminExists)
            {
                return; // Admin already exists, skip seeding
            }

            // Get admin settings from configuration
            var adminSection = configuration.GetSection("AdminSettings");
            
            // Override with environment variables if they exist, otherwise use appsettings.json, otherwise use defaults
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") 
                ?? adminSection["Email"] 
                ?? "admin@cms.com";
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") 
                ?? adminSection["Password"] 
                ?? "Admin@123";
            var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME") 
                ?? adminSection["Name"] 
                ?? "System Administrator";
            var adminPhone = Environment.GetEnvironmentVariable("ADMIN_PHONE") 
                ?? adminSection["Phone"] 
                ?? "1234567890";

            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

            // Create admin user
            var adminUser = new User
            {
                UserID = Guid.NewGuid(),
                Name = adminName,
                Email = adminEmail,
                PhoneNumber = adminPhone,
                PasswordHash = passwordHash,
                Role = RoleType.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        public static async Task SeedAsync(CmsDbContext context, IConfiguration configuration)
        {
            await SeedAdminUserAsync(context, configuration);
        }
    }
}

