using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MSDF.StudentEngagement.Persistence.EntityFramework;
using MSDF.StudentEngagement.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Persistence.CommandsAndQueries.Queries
{
    public interface ILearningAppQueries
    {
        Task<LearningApp> GetWhitelistedApp(string url);
        Task<List<LearningApp>> GetAll();
    }

    public class LearningAppQueries : ILearningAppQueries
    {
        private readonly DatabaseContext _db;
        public LearningAppQueries(DatabaseContext db)
        {
            this._db = db;
        }

        public async Task<List<LearningApp>> GetAll()
        {
            return await _db.LearningApps
                .Where(la => la.TrackingEnabled)
                .ToListAsync();
        }

        public async Task<LearningApp> GetWhitelistedApp(string url)
        {
            var lApps = await _db.LearningApps
                .ToListAsync();
            
            var lApp = lApps
                .Where(la => Regex.IsMatch(url, la.WhitelistRegex) && la.TrackingEnabled)
                .FirstOrDefault();
            return lApp;
        }
    }
}
