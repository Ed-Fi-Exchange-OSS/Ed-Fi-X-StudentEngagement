using Microsoft.EntityFrameworkCore;
using MSDF.StudentEngagement.Persistence.Models;

namespace MSDF.StudentEngagement.Persistence.EntityFramework
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<StudentLearningEventLog> StudentLearningEventLogs { get; set; }
        public DbSet<StudentInformation> StudentInformation { get; set; }
        public DbSet<LearningApp> LearningApps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentLearningEventLog>().ToTable("StudentLearningEventLog");
            modelBuilder.Entity<StudentInformation>().ToTable("StudentInformation");
            modelBuilder.Entity<LearningApp>().ToTable("LearningApp");

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LearningApp>().HasData(
                new LearningApp[] 
                {
                    new LearningApp()
                    {
                        LearningAppIdentifier = "schoology",
                        AppUrl = "schoology.com",
                        WhitelistRegex = @"^.*\.schoology.com\/.*",
                        TrackingEnabled = true
                    } 
                });
        }
    }
}
