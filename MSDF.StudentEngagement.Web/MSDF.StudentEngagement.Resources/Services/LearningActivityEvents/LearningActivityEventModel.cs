using System;

namespace MSDF.StudentEngagement.Resources.Services.LearningActivityEvents
{
    public class LearningActivityEventModel
    {
        public string IdentityElectronicMailAddress { get; set; }
        public string IPAddress { get; set; }
        // Learning Event Details
        public string ReffererUrl { get; set; }
        public string LeaningAppUrl { get; set; }
        public DateTime UTCStartDateTime { get; set; }
        public DateTime UTCEndDateTime { get; set; }
    }
}
