﻿using MSDF.StudentEngagement.Persistence.Models;
using MSDF.StudentEngagement.Persistence.StudentLearningEvent.Commands;
using System;
using System.Collections.Generic;
using System.Text;
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

        public LearningActivityEventsService(IStudentLearningEventLogCommands studentLearningEventLogCommands)
        {
            this._studentLearningEventLogCommands = studentLearningEventLogCommands;
        }

        public async Task SaveLearningActivityEventAsync(LearningActivityEventModel model)
        {
            StudentLearningEventLog entity = MapToStudentLearningEventLog(model);

            await _studentLearningEventLogCommands.AddAsync(entity);
        }

        public async Task SaveLearningActivityEventAsync(List<LearningActivityEventModel> modelList)
        {
            foreach (var model in modelList)
            {
                var entity = MapToStudentLearningEventLog(model);
                await _studentLearningEventLogCommands.AddorUpdateAsync(entity);
            }
        }

        private static StudentLearningEventLog MapToStudentLearningEventLog(LearningActivityEventModel model)
        {
            // Map model to entity
            var entity = new StudentLearningEventLog
            {
                StudentElectronicMailAddress = model.IdentityElectronicMailAddress,
                LeaningAppUrl = model.LeaningAppUrl,
                UTCStartDate = model.UTCStartDateTime,
                UTCEndDate = model.UTCEndDateTime
                // TODO: Add other properties as needed.
            };
            if (model.UTCEndDateTime != null) { entity.TimeSpent = (int?)(entity.UTCEndDate.Value - entity.UTCStartDate).TotalSeconds; }
            return entity;
        }

    }
}
