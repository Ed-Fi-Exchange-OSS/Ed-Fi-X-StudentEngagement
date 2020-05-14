using Microsoft.EntityFrameworkCore;
using MSDF.StudentEngagement.Persistence.Models;

namespace MSDF.StudentEngagement.Persistence.EntityFramework
{
    public class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public virtual DbSet<StudentLearningEventLog> StudentLearningEventLogs { get; set; }


    }
}
