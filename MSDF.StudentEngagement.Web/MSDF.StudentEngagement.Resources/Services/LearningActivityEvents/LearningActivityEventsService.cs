using MSDF.StudentEngagement.Persistence.CommandsAndQueries.Commands;
using MSDF.StudentEngagement.Persistence.CommandsAndQueries.Queries;
using MSDF.StudentEngagement.Persistence.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Resources.Services.LearningActivityEvents
{
    public interface ILearningActivityEventsService {
        Task SaveLearningActivityEventAsync(LearningActivityEventModel model);
        Task SaveLearningActivityEventAsync(List<LearningActivityEventModel> modelList);
    }
    public class LearningActivityEventsService : ILearningActivityEventsService
    {
        private readonly IStudentLearningEventLogCommands _studentLearningEventLogCommands;
        private readonly ILearningAppQueries _learningAppQueries;

        public LearningActivityEventsService(
            IStudentLearningEventLogCommands studentLearningEventLogCommands,
            ILearningAppQueries learningAppQueries)
        {
            this._studentLearningEventLogCommands = studentLearningEventLogCommands;
            this._learningAppQueries = learningAppQueries;
        }

        public async Task SaveLearningActivityEventAsync(LearningActivityEventModel model)
        {
            var learningApp = await _learningAppQueries.GetWhitelistedApp(model.LeaningAppUrl);
            StudentLearningEventLog entity = MapToStudentLearningEventLog(model, learningApp);
            await _studentLearningEventLogCommands.AddAsync(entity);
        }

        public async Task SaveLearningActivityEventAsync(List<LearningActivityEventModel> modelList)
        {
            foreach (var model in modelList)
            {
                var whitelistedApp = await _learningAppQueries.GetWhitelistedApp(model.LeaningAppUrl);
                if (null == whitelistedApp) { continue; }
                var entity = MapToStudentLearningEventLog(model, whitelistedApp);
                await _studentLearningEventLogCommands.AddorUpdateAsync(entity);
            }
        }

        private static StudentLearningEventLog MapToStudentLearningEventLog(
            LearningActivityEventModel model, 
            Persistence.Models.LearningApp learningApp)
        {
            if (null == learningApp) return null;

            // Map model to entity
            var entity = new StudentLearningEventLog
            {
                StudentElectronicMailAddress = model.IdentityElectronicMailAddress,
                LeaningAppUrl = model.LeaningAppUrl,
                UTCStartDate = model.UTCStartDateTime,
                UTCEndDate = model.UTCEndDateTime,
                LearningAppIdentifier = learningApp.LearningAppIdentifier
                // TODO: Add other properties as needed.
            };
            if (model.UTCEndDateTime != null) { entity.TimeSpent = (int?)(entity.UTCEndDate.Value - entity.UTCStartDate).TotalSeconds; }
            return entity;
        }

    }
}
