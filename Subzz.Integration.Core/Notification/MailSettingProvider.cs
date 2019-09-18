using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Subzz.Integration.Core.Domain;
using SubzzV2.Integration.Core.Notification.Interface;
using Dapper;

namespace SubzzV2.Integration.Core.Notification
{
	public class MailSettingProvider : ISettingProvider<MailSettings>
	{
		public MailSettings GetSetting()
		{
			try
			{
				//string connectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
				List<MailSettings> mailSettings;
                using (var connection = new SqlConnection("Data Source=DESKTOP-BQ3LKMC\\SQLEXPRESS;Initial Catalog=Subzz_Settings;Integrated Security=True"))
                //using (var connection = new SqlConnection("data source=162.241.138.178;Initial Catalog=Subzz_Settings;user id=subzz_user;password=w9if%l10;multipleactiveresultsets=true"))
                //using (var connection = new SqlConnection("data source=162.241.138.178\\stg;Initial Catalog=Subzz_Settings;user id=tamoor;password=password;multipleactiveresultsets=true"))
                {
                    mailSettings = connection.Query<MailSettings>(@"
					SELECT 
					[MailSettings_awsSecretAccessKey] as AwsSecretAccessKey, 
					[MailSettings_awsAccessKeyId] as AwsAccessKeyId
					FROM [Settings].[MailSettings] WHERE [IsActive] = 1").ToList();
                }
				return mailSettings.FirstOrDefault();
			}
			catch (SqlException ex)
			{
				throw ex;
				//TODO: log error
			}
			return null;
		}
	}
}