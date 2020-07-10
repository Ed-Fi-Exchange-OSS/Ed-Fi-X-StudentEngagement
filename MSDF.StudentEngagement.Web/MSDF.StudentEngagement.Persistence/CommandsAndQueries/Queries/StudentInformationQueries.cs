using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MSDF.StudentEngagement.Persistence.EntityFramework;
using MSDF.StudentEngagement.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Persistence.CommandsAndQueries.Queries
{
    public interface IStudentInformationQueries
    {
        Task<StudentInformation> GetStudentFromEmail(string email);
        
    }

    public class StudentInformationQueries : IStudentInformationQueries
    {
        private readonly DatabaseContext _db;
        public StudentInformationQueries(DatabaseContext db)
        {
            this._db = db;
        }

        public Task<StudentInformation> GetStudentFromEmail(string email)
        {
            return _db.StudentInformation
                .Where(si => si.IdentityElectronicMailAddress == email )
                .FirstOrDefaultAsync();
        }
    }
}