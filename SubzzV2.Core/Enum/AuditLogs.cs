namespace SubzzV2.Core.Enum
{
    public static class AuditLogs
    {
        public static class EntityType
        {
            public static string User       = "User";
            public static string Absence    = "Absence";
            public static string District   = "District";
            public static string School     = "School";
            public static string Staff      = "Staff";
            public static string Substitute = "Substitute";
            public static string LeaveType  = "LeaveType";
            public static string Allowances = "Allowances";
            public static string ChangedPassword = "ChangedPassword";
            public static string PayRate         = "PayRate";
            public static string PayRateRule     = "PayRateRule";
            public static string Unavailability  = "Unavailability";
            public static string Announcement    = "Unavailability";
        }

        public static class ActionType
        {
            public static string Create     = "Created";
            public static string Update     = "Updated";
            public static string Delete     = "Deleted";
            public static string Cancelled  = "Cancelled";
            public static string Viewed     = "Viewed";
            public static string Accepted   = "Accepted";
            public static string Declined   = "Declined";
            public static string Released   = "Released";
            public static string Assigned   = "Assigned";
            public static string Approved   = "Approved";
            public static string Denied     = "Denied";
            public static string Archived   = "Archived";

            // For User Related Actions
            public static string LoggedIn        = "Logged In";
            public static string LoggedOut       = "Logged Out";
            public static string UpdatedProfile  = "Updated Profile Information";
            public static string ChangedPassword = "Changed Password For User";
            public static string SubstituteFile  = "Added Substitute File";
            public static string TGuideForAdmin  = "Added Training Guide For Admins";
            public static string TGuideForStaff  = "Added Training Guide For Staff";
            public static string TGuideForSub    = "Added Training Guide For Substitutes";
            public static string DeletedSubstituteFile      = "Deleted Substitute File";
            public static string DeletedGuide               = "Deleted Training Guide";
            public static string UpdatedNotifySettings      = "Updated Notification Settings";
            public static string UpdatedSchoolSettings      = "Updated School Settings";
            public static string UpdatedCategorySettings    = "Updated Categories Settings";
            public static string UpdatedSubPreference       = "Updated Substitute Preferences";

            // For District Related Actions
            public static string CreatedDistrict    = "Added District";
            public static string UpdatedDistrict    = "Updated District";
            public static string DeletedDistrict    = "Deleted District";
            public static string ViewedDistrict     = "Viewed District";

            // For Organization/School Related Actions
            public static string CreatedSchool      = "Added School";
            public static string UpdatedSchool      = "Updated School";
            public static string DeletedSchool      = "Deleted School";
            public static string ViewedSchool       = "Viewed School";

            // For Employee/Staff Related Actions
            public static string CreatedEmployee    = "Added Employee";
            public static string UpdatedEmployee    = "Updated Employee";
            public static string DeletedEmployee    = "Deleted Employee";
            public static string ViewedEmployee     = "Viewed Employee";
            public static string EmployeeActive     = "Activated Employee";
            public static string EmployeeInactive   = "Inactivated Employee";

            // For Substitute Related Actions
            public static string CreatedSubstitute  = "Added Substitute";
            public static string UpdatedSubstitute  = "Updated Substitute";
            public static string DeletedSubstitute  = "Deleted Substitute";
            public static string ViewedSubstitute   = "Viewed Substitute";
            public static string SubstituteeActive  = "Activated Substitute";
            public static string SubstituteInactive = "Inactivated Substitute";
            public static string CreatedSubPosition = "Added Substitute Position";
            public static string UpdatedSubPosition = "Updated Substitute Position";
            public static string DeletedSubPosition = "Deleted Substitute Position";
            public static string UpdatedSubList     = "Updated School Substitute List";
            public static string UpdatedBlockedList = "Updated School Blocked Substitute List";

            // For LeaveType Related Actions
            public static string CreatedLeaveType   = "Added Leave Type";
            public static string UpdatedLeaveType   = "Updated Leave Type";
            public static string DeletedLeaveType   = "Deleted Leave Type";
            public static string ViewedLeaveType    = "Viewed Leave Type";

            // For Allowances Related Actions
            public static string CreatedAllowance  = "Added Allowance";
            public static string UpdatedAllowance  = "Updated Allowance";
            public static string DeletedAllowance  = "Deleted Allowance";
            public static string ViewedAllowance   = "Viewed Allowance";

            // For Pay Rate Related Actions
            public static string CreatedPayRate     = "Added Pay Rate";
            public static string UpdatedPayRate     = "Updated Pay Rate";
            public static string DeletedPayRate     = "Deleted Pay Rate";
            public static string ViewedPayRate      = "Viewed Pay Rate";

            // For Pay Rate Rule Related Actions
            public static string CreatedPayRateRule = "Added Pay Rate Rule";
            public static string UpdatedPayRateRule = "Updated Pay Rate Rule";
            public static string DeletedPayRateRule = "Deleted Pay Rate Rule";
            public static string ViewedPayRateRule  = "Viewed Pay Rate Rule";

            // For Availability/Unavailability Related Actions
            public static string CreatedUnavailability = "Added Unavailability";
            public static string UpdatedUnavailability = "Updated Unavailability";
            public static string DeletedUnavailability = "Deleted Unavailability";

            // For Announcement Related Actions
            public static string CreatedAnnouncement = "Added Announcement";
            public static string UpdatedAnnouncement = "Updated Announcement";
            public static string DeletedAnnouncement = "Deleted Announcement";
        }
    }
}
