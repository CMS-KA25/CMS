using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CMS.Data
{
    public class CmsDbContextFactory : IDesignTimeDbContextFactory<CmsDbContext>
    {
        public CmsDbContext CreateDbContext(string[] args)
        {
            // Load environment variables from .env file if it exists
            DotNetEnv.Env.Load();

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                ?? "Server=localhost;Database=CMS_DB_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

            var optionsBuilder = new DbContextOptionsBuilder<CmsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new CmsDbContext(optionsBuilder.Options);
        }
    }
}

