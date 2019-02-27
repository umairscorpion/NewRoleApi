using Dapper;
using Subzz.Integration.Core.Domain;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace SubzzV2.Integration.Core.Notification
{
    public class SmsMessagingSetting : ISettingProvider<MessagingSetting>
    {
        public MessagingSetting GetSetting()
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
            List<MessagingSetting> messagingSettings;
            //using (var dc = new DataContext(connectionString))
            //{
            //	messagingSettings = dc.ExecuteQuery<MessagingSetting>(@"
            //		SELECT TOP 1 
            //			 [AccountSid]
            //			,[AuthToken]
            //                     ,[SenderPhoneNumber]
            //                     ,[isActive]
            //			FROM [support].[SmsMessagingSettings] WHERE [IsActive] = {0}", "1").ToList();
            //}
            using (var dc = new SqlConnection(""))
            {
                messagingSettings = dc.Query<MessagingSetting>(@"
					SELECT  
						 [AccountSid]
						,[AuthToken]
                        ,[SenderPhoneNumber]
                        ,[isActive]
						FROM [support].[SmsMessagingSettings] where isActive = 1").ToList();
            }
            return messagingSettings.FirstOrDefault();
        }
    }
}