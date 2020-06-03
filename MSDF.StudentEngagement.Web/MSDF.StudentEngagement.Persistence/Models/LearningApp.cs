using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.StudentEngagement.Persistence.Models
{
    public class LearningApp
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [StringLength(60)]
        public string LearningAppIdentifier { get; set; }
        [StringLength(255)]
        public string Namespace { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }
        [StringLength(255)]
        public string Website { get; set; }
        [StringLength(255)]
        public string AppUrl { get; set; }
        [StringLength(255)]
        public string WhitelistRegex { get; set; }
        public bool TrackingEnabled { get; set; }        
    }
}
