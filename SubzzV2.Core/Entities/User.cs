using System;
using SubzzV2.Core.Models;
using System.Collections.Generic;

namespace SubzzV2.Core.Entities
{
    public class User
    {
        public User()
        {
            Permissions = new List<RolePermission>();
            SecondarySchools = new List<string>();
        }

        public string Id
        {
            get; set;
        }
        public string UserId { get; set; }
        public int UserStatusId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSubscribedSMS { get; set; }
        public bool IsSubscribedEmail { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int UserTypeId { get; set; }
        public string Description { get; set; }
        public string ActivationCode { get; set; }
        public UserProfile Profile { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UserIP { get; set; }
        public string UserBrowser { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string Device { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int IsCertified { get; set; }
        public string Gender { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public bool Isdeleted { get; set; }
        public string UserTypeDescription { get; set; }
        public string UserRoleDesciption { get; set; }
        public string Speciality { get; set; }
        public int SpecialityTypeId { get; set; }
        public int TeachingLevel { get; set; }
        public int UserLevel { get; set; }
        public int CategoryId { get; set; }
        public string Password { get; set; }
        public int PayRate { get; set; }
        public int HourLimit { get; set; }
        public string LastActive { get; set; }
        public List<string> SecondarySchools = new List<string>();
        public List<RolePermission> Permissions { get; set; }
        public DateTime StartDate { get; set; }
        public int JobNumber { get; set; }
        public int SearchType { get; set; }
        public int ReportType { get; set; }
        public bool IsViewedNewVersion { get; set; }
        public bool ForUserVerification { get; set; }
        public string CounrtyCode { get; set; }
    }

    public class UserSummary {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
