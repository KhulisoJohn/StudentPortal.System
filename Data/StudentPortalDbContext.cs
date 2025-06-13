using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Data
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
                .HasKey(ss => new { ss.StudentId, ss.SubjectId });

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentId);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId);

            // --- TutorSubject (Many-to-Many) ---
            modelBuilder.Entity<TutorSubject>()
                .HasKey(ts => new { ts.TutorId, ts.SubjectId });

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.Tutor)
                .WithMany(t => t.TutorSubjects)
                .HasForeignKey(ts => ts.TutorId);

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.Subject)
                .WithMany(s => s.TutorSubjects)
                .HasForeignKey(ts => ts.SubjectId);

            // --- UserChatChannel (Many-to-Many) ---
            modelBuilder.Entity<UserChatChannel>()
                .HasKey(uc => new { uc.UserId, uc.ChatChannelId });

            modelBuilder.Entity<UserChatChannel>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatChannels)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserChatChannel>()
                .HasOne(uc => uc.ChatChannel)
                .WithMany(cc => cc.UserChatChannels)
                .HasForeignKey(uc => uc.ChatChannelId);

            // --- ChatMessage Relationships ---
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.UserId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatChannel)
                .WithMany(cc => cc.ChatMessages)
                .HasForeignKey(cm => cm.ChatChannelId);

            // --- One-to-One: Student <-> ApplicationUser ---
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Student)          // <--- change from StudentProfile to Student
                .HasForeignKey<Student>(s => s.ApplicationUserId);

            modelBuilder.Entity<Tutor>()
                .HasOne(t => t.ApplicationUser)
                .WithOne(u => u.Tutor)            // <--- change from TutorProfile to Tutor
                .HasForeignKey<Tutor>(t => t.ApplicationUserId);

        }
    }
}
