using System;

namespace MSDF.StudentEngagement.Resources.Services.LearningActivityEvents
{
    public class LearningActivityEventModel
    {
        public string IdentityElectronicMailAddress { get; set; }
        // Learning Event Logging
        public string ReffererUrl { get; set; }
        public string LeaningAppUrl { get; set; }
        public DateTime UTCDateTimeStart { get; set; }
        public DateTime UTCDateTimeEnd { get; set; }
    }
}
