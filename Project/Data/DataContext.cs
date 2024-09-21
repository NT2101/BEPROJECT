using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
      : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<TopicChangeRequest> TopicChangeRequests { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<CommitteeTeacherMember> CommitteeTeacherMembers { get; set; }
        public DbSet<CommitteeStudentMember> CommitteeStudentMembers { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define foreign keys
            modelBuilder.Entity<Specialization>()
                .HasOne(s => s.Faculty)
                .WithMany(f => f.Specializations)
                .HasForeignKey(s => s.FacultyID);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Specialization)
                .WithMany(s => s.Classes)
                .HasForeignKey(c => c.SpecializationID);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassID);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentID);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherID);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Field)
                .WithMany()
                .HasForeignKey(t => t.FieldID);

            modelBuilder.Entity<RegistrationRequest>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId);

            modelBuilder.Entity<RegistrationRequest>()
                         .HasOne(t => t.Teacher)
                         .WithMany()
                         .HasForeignKey(t => t.TeacherId);

         
    }

}
}
