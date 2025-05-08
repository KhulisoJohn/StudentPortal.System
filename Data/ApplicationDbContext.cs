using System;
using Microsoft.EntityFrameworkCore; // ✅ Required for EF Core
using StudentPortal.Models.Entities;


namespace StudentPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }  
         public DbSet<Student> Students { get; set; } // ✅ Corrected casing
    }
}