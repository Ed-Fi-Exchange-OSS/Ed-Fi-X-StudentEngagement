using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MSDF.StudentEngagement.Persistence.EntityFramework;
using MSDF.StudentEngagement.Persistence.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Persistence.StudentLearningEvent.Commands
{
    public interface IStudentLearningEventLogCommands
    {
        Task<StudentLearningEventLog> AddAsync(StudentLearningEventLog model);
        Task<StudentLearningEventLog> AddorUpdateAsync(StudentLearningEventLog model);
    }
    public class StudentLearningEventLogCommands : IStudentLearningEventLogCommands
    {
        private readonly DatabaseContext _db;

        public StudentLearningEventLogCommands(DatabaseContext db)
        {
            this._db = db;
        }

        public async Task<StudentLearningEventLog> AddAsync(StudentLearningEventLog model)
        {
            _db.Add(model);
            await _db.SaveChangesAsync();
            return model;
        }
        public async Task<StudentLearningEventLog> AddorUpdateAsync(StudentLearningEventLog model)
        {
            var id = _db.StudentLearningEventLogs
                .Where(el =>
                    el.StudentElectronicMailAddress == model.StudentElectronicMailAddress &&
                    el.LeaningAppUrl == model.LeaningAppUrl &&
                    el.UTCStartDate == model.UTCStartDate 
                )
                .Select(el => el.Id)
                .FirstOrDefault();

            if (id != 0)
            {
                model.Id = id;
                _db.Attach(model);
                _db.Entry(model).State = EntityState.Modified;
            }
            else
            {
                _db.Add(model);
            }
            await _db.SaveChangesAsync();
            return model;
        }

    }
}
