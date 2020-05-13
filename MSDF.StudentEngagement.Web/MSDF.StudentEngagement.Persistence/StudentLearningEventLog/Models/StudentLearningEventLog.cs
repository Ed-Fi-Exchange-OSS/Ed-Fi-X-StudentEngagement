using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;

namespace MSDF.StudentEngagement.Persistence.StudentLearningEventLog.Models
{
    public class StudentLearningEventLog
    {
        [Key]
        public BigInteger Id { get; set; }

        // Student Identification Options
        public int StudentUSI { get; set; }
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [StringLength(32)]
        public string DeviceId { get; set; }
        [StringLength(128)]
        public string StudentElectronicMailAddress { get; set; }
        [StringLength(15)]
        public string IPAddress { get; set; }

        // Learning Event Logging
        [StringLength(1024)]
        public string ReffererUrl { get; set; }
        [StringLength(1024)]
        public string LeaningAppUrl { get; set; }
        [Required]
        public DateTime UTCDateTimeStart { get; set; }
        public DateTime UTCDateTimeEnd { get; set; }
        public int DurationInSeconds { get; set; }




        //`StudentUSI` int not null,
        //`LearningAppId` int not null,
        //`Url` varchar(1024) not null,
        //`ReffererUrl` varchar(1024) not null,
        //`StartTime` datetime not null,
        //`EndTime` datetime null,
        //`DurationMS` int null,
    }
}
