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
            List<MessagingSetting> messagingSettings;
            using (var connection = new SqlConnection("Data Source=DESKTOP-THR93CO\\SQLEXPRESS;Initial Catalog=Subzz_Settings;Integrated Security=True"))
            //using (var connection = new SqlConnection("data source=162.241.138.178;Initial Catalog=Subzz_Settings;user id=subzz_user;password=w9if%l10;multipleactiveresultsets=true"))
            //using (var connection = new SqlConnection("data source=162.241.138.178\\stg;Initial Catalog=Subzz_Settings;user id=tamoor;password=password;multipleactiveresultsets=true"))
            {
                messagingSettings = connection.Query<MessagingSetting>(@"
					SELECT  
						 [AccountId]
						,[AuthToken]
                        ,[SenderPhoneNumber]
                        ,[isActive]
						FROM [Subzz_Settings].[Settings].[SmsMessagingSettings] where isActive = 1").ToList();
            }
            return messagingSettings.FirstOrDefault();
        }
    }
}