using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Models;

namespace StudentPortalSystem.Data
{
    public class StudentPortalDbContext : IdentityDbContext<ApplicationUser>
    {
        public StudentPortalDbContext(DbContextOptions<StudentPortalDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<TutorSubject> TutorSubjects { get; set; }
        public DbSet<ChatChannel> ChatChannels { get; set; }
        public DbSet<TutorMaterial> TutorMaterials { get; set; }
        public DbSet<UserChatChannel> UserChatChannels { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- StudentSubject (Many-to-Many) ---
            modelBuilder.Entity<StudentSubject>()
                .HasKey(static ss => new { ss.StudentId, ss.SubjectId });

            modelBuilder.Entity<StudentSubject>()
                .HasOne(static ss => ss.Student)
                .WithMany(static s => s.StudentSubjects)
                .HasForeignKey(static ss => ss.StudentId);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(static ss => ss.Subject)
                .WithMany(static s => s.StudentSubjects)
                .HasForeignKey(static ss => ss.SubjectId);

            // --- TutorSubject (Many-to-Many) ---
            modelBuilder.Entity<TutorSubject>()
                .HasKey(static ts => new { ts.TutorId, ts.SubjectId });

            modelBuilder.Entity<TutorSubject>()
                .HasOne(static ts => ts.Tutor)
                .WithMany(static t => t.TutorSubjects)
                .HasForeignKey(static ts => ts.TutorId);

            modelBuilder.Entity<TutorSubject>()
                .HasOne(static ts => ts.Subject)
                .WithMany(static s => s.TutorSubjects)
                .HasForeignKey(static ts => ts.SubjectId);

            // --- UserChatChannel (Many-to-Many) ---
            modelBuilder.Entity<UserChatChannel>()
                .HasKey(static uc => new { uc.UserId, uc.ChatChannelId });

            modelBuilder.Entity<UserChatChannel>()
                .HasOne(static uc => uc.User)
                .WithMany(static u => u.UserChatChannels)
                .HasForeignKey(static uc => uc.UserId);

            modelBuilder.Entity<UserChatChannel>()
                .HasOne(static uc => uc.ChatChannel)
                .WithMany(static cc => cc.UserChatChannels)
                .HasForeignKey(static uc => uc.ChatChannelId);

            // --- ChatMessage Relationships ---
            modelBuilder.Entity<ChatMessage>()
                .HasOne(static cm => cm.User)
                .WithMany(static u => u.ChatMessages)
                .HasForeignKey(static cm => cm.UserId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(static cm => cm.ChatChannel)
                .WithMany(static cc => cc.ChatMessages)
                .HasForeignKey(static cm => cm.ChatChannelId);

            // --- One-to-One: Student <-> ApplicationUser ---
            modelBuilder.Entity<Student>()
                .HasOne(static s => s.ApplicationUser)
                .WithOne(static u => u.Student)          // <--- change from StudentProfile to Student
                .HasForeignKey<Student>(static s => s.ApplicationUserId);

            modelBuilder.Entity<Tutor>()
                .HasOne(static t => t.ApplicationUser)
                .WithOne(static u => u.Tutor)            // <--- change from TutorProfile to Tutor
                .HasForeignKey<Tutor>(static t => t.ApplicationUserId);

        }
    }
}
