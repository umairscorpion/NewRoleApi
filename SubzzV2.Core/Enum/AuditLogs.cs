namespace SubzzV2.Core.Enum
{
    public static class AuditLogs
    {
        public static class EntityType
        {
            public static string User = "User";
            public static string Absence = "Absence";
        }

        public static class ActionType
        {
            public static string Create = "Created";
            public static string Update = "Updated";
            public static string Delete = "Deleted";
            public static string Cancelled = "Cancelled";
            public static string Viewed = "Viewed";
            public static string Accepted = "Accepted";
            public static string Declined = "Declined";
            public static string Release = "Released";
            public static string Assigned = "Assigned";
            public static string Approved = "Approved";


            public static string LoggedIn = "LoggedIn";
            public static string LoggedOut = "LoggedOut";
        }
    }
}
