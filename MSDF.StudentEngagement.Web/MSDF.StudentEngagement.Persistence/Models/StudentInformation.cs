using System;
using System.ComponentModel.DataAnnotations;

namespace MSDF.StudentEngagement.Persistence.Models
{
    public class StudentInformation
    {
        /*Student Identity*/
        [Key]
        public int Id { get; set; }
        public int StudentUSI { get; set; }
        [StringLength(32)]
        public string StudentUniqueId{ get; set; }
        [StringLength(60)]
        public string StudentStateIdentificationCode { get; set; }
        [StringLength(60)]
        public string IdentityElectronicMailAddress { get; set; }
        [StringLength(100)]
        public string DeviceId { get; set; } 
	
	    /*enrollment*/
        [StringLength(75)]
	    public string LocalEducationAgencyName { get; set; }
        [StringLength(75)] 
        public string SchoolName { get; set; }
        [StringLength(15)] 
        public string SchoolYear { get; set; } /*Ex. 2019-2020{ get; set; } 2019 - 2020*/
        [StringLength(50)]
        public string SchoolCurrentGradeLevelDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string SchoolTypeDescriptorCodeValue { get; set; }
	    public DateTime? ExitWithdrawalDate { get; set; }

        /*student info*/
        [StringLength(75)]
        public string FirstName { get; set; }
        [StringLength(75)]
        public string MiddleName { get; set; }
        [StringLength(75)]
        public string LastSurname { get; set; }
	    public DateTime BirthDate { get; set; }
        [StringLength(50)]
        public string BirthSexDescriptorCodeValue { get; set; }

        /*race and ethnicities*/
        [StringLength(25)]
        public string Ethnicity { get; set; }
	    public bool? Race_AmericanIndianAlaskanNative { get; set; }
	    public bool? Race_Asian { get; set; }
	    public bool? Race_BlackAfricaAmerican { get; set; }
	    public bool? Race_NativeHawaiianPacificIslander { get; set; }
	    public bool? Race_White { get; set; }
	    public bool? Race_ChooseNotToRespond  { get; set; }
	    public bool? Race_Other { get; set; }
        
        [StringLength(50)]
        public string DisabilityStatusDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string EconomicallyDisadvantageDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string ELLStatusDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string MigrantDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string HomelessDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string FosterDescriptorCodeValue { get; set; }
        [StringLength(50)]
        public string F504DescriptorCodeValue { get; set; }
        /*contact info*/

        [StringLength(75)]
        public string ContactInfoFirstName { get; set; }

        [StringLength(75)]
        public string ContactInfoLastSurname { get; set; }

        [StringLength(75)]
        public string ContactInfoRelationToStudent { get; set; }

        [StringLength(20)]
        public string ContactInfoCellPhoneNumber { get; set; }

        [StringLength(60)]
        public string ContactInfoElectronicMailAddress { get; set; }

    }
}
