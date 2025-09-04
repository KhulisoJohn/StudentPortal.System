using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace StudentPortalSystem.Data
{
    public class StudentPortalDbContextFactory : IDesignTimeDbContextFactory<StudentPortalDbContext>
    {
        public StudentPortalDbContext CreateDbContext(string[] args)
        {
            // 🔹 Load environment variables from .env file
            Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            // 🔹 Read variables
            var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (string.IsNullOrEmpty(dbServer) ||
                string.IsNullOrEmpty(dbPort) ||
                string.IsNullOrEmpty(dbName) ||
                string.IsNullOrEmpty(dbUser) ||
                string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("Missing one or more database environment variables.");
            }

            // 🔹 Build PostgreSQL connection string
            var connectionString = $"Host={dbServer};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

            var optionsBuilder = new DbContextOptionsBuilder<StudentPortalDbContext>();
            optionsBuilder.UseNpgsql(connectionString); // Use PostgreSQL

            return new StudentPortalDbContext(optionsBuilder.Options);
        }
    }
}
