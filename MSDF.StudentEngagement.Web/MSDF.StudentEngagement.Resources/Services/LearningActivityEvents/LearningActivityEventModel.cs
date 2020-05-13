using System;
using System.Collections.Generic;
using System.Text;

namespace MSDF.StudentEngagement.Resources.Services.LearningActivityEvents
{
    public class LearningActivityEventModel
    {
        public string IdentityEmailAddress { get; set; }
        // Learning Event Logging
        public string ReffererUrl { get; set; }
        public string LeaningAppUrl { get; set; }
        public DateTime UTCDateTimeStart { get; set; }
    }
}
