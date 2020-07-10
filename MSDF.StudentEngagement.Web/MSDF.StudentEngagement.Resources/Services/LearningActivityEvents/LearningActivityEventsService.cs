using MSDF.StudentEngagement.Persistence.CommandsAndQueries.Commands;
using MSDF.StudentEngagement.Persistence.CommandsAndQueries.Queries;
using MSDF.StudentEngagement.Persistence.Models;
using System;
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
        private readonly IStudentInformationQueries _studentInformationQueries;
        private readonly ILearningAppQueries _learningAppQueries;

        public LearningActivityEventsService(
            IStudentLearningEventLogCommands studentLearningEventLogCommands,
            IStudentInformationQueries studentInformationQueries,
            ILearningAppQueries learningAppQueries)
        {
            this._studentLearningEventLogCommands = studentLearningEventLogCommands;
            this._studentInformationQueries = studentInformationQueries;
            this._learningAppQueries = learningAppQueries;
        }

        public async Task SaveLearningActivityEventAsync(LearningActivityEventModel model)
        {
            var learningApp = await _learningAppQueries.GetWhitelistedApp(model.LeaningAppUrl);
            StudentLearningEventLog entity = await MapToStudentLearningEventLog(model, learningApp);
            await _studentLearningEventLogCommands.AddAsync(entity);
        }

        public async Task SaveLearningActivityEventAsync(List<LearningActivityEventModel> modelList)
        {
            foreach (var model in modelList)
            {
                var whitelistedApp = await _learningAppQueries.GetWhitelistedApp(model.LeaningAppUrl);
                if (null == whitelistedApp) { continue; }
                var entity = await MapToStudentLearningEventLog(model, whitelistedApp);
                await _studentLearningEventLogCommands.AddorUpdateAsync(entity);
            }
        }

        private async Task<StudentLearningEventLog> MapToStudentLearningEventLog(
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
            await setStudentIds(entity);
            if (model.UTCEndDateTime != null) { entity.TimeSpent = (int?)(entity.UTCEndDate.Value - entity.UTCStartDate).TotalSeconds; }
            return entity;
        }

        private async Task setStudentIds(StudentLearningEventLog entity)
        {
            var student = await _studentInformationQueries.GetStudentFromEmail(entity.StudentElectronicMailAddress);
            if (null == student) { return; }
            entity.StudentUSI = student.StudentUSI;
            entity.StudentUniqueId = student.StudentUniqueId;
        }
    }
}
