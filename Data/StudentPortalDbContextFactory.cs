using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StudentPortalSystem.Data;

namespace StudentPortalSystem.Data
{
    public class StudentPortalDbContextFactory : IDesignTimeDbContextFactory<StudentPortalDbContext>
    {
        public StudentPortalDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StudentPortalDbContext>();
            var connectionString = "Server=127.0.0.1;Port=3306;Database=StudentPortalDb;User=root;Password=Khulyso@10;";
            
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new StudentPortalDbContext(optionsBuilder.Options);
        }
    }
}
