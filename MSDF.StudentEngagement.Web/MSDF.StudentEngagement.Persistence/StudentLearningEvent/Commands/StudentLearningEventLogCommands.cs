using MSDF.StudentEngagement.Persistence.EntityFramework;
using MSDF.StudentEngagement.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Persistence.StudentLearningEvent.Commands
{
    public interface IStudentLearningEventLogCommands {
        Task<StudentLearningEventLog> Add(StudentLearningEventLog model);
    }
    public class StudentLearningEventLogCommands : IStudentLearningEventLogCommands
    {
        private readonly DatabaseContext _db;

        public StudentLearningEventLogCommands(DatabaseContext db) 
        {
            this._db = db;
        }

        public async Task<StudentLearningEventLog> Add(StudentLearningEventLog model) 
        {
            _db.Add(model);
            await _db.SaveChangesAsync();
            return model;
        }

    }
}
