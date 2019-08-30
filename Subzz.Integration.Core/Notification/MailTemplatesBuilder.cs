using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Threading.Tasks;
using Subzz.Integration.Core.Domain;
using Dapper;

namespace SubzzV2.Integration.Core.Notification
{
	public class MailTemplatesBuilder
	{
		public MailTemplates GetMailTemplates()
		{
			//string connectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
			List<MailTemplate> mailTemplatesList;
			using (var dc = new SqlConnection(""))
			{
				mailTemplatesList = dc.Query<MailTemplate>(@"
					SELECT
						[Id]
						,[UserType]
						,[Title]
						,[EmailContent]
						,[EmailDisclaimerNeeded]
						,[TextContent]
						,[Notes]
						,[SendTo]
						,[EmailDisclaimerContent]
						,[SupportEmail]
					FROM [support].[MailTemplates]").ToList();
			}
			var mailTemplates = new MailTemplates();
			mailTemplatesList.ForEach(e => mailTemplates.Add(e));
			return mailTemplates;
		}
		public MailTemplate GetMailTemplateById(int id)
		{
			//string connectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
			List<MailTemplate> mailTemplatesList;
			using (var dc = new SqlConnection(""))
			{
				mailTemplatesList = dc.Query<MailTemplate>(@"
					SELECT
						[Id]
						,[Title]
						,[EmailContent]
						,[EmailDisclaimerNeeded]
						,[TextContent]
						,[Notes]
						,[SendTo]
						,[SenderEmail]
					
						,[EmailDisclaimerContent]
						,[SupportEmail]
					FROM [support].[MailTemplates]
					WHERE [Id] = {0}", id).ToList();
			}
			if(!mailTemplatesList.Any())
			{
				throw new ArgumentException(string.Format("Template with specified id {0} doesn't exist", id), "id");
			}
			return mailTemplatesList.FirstOrDefault();
		}

        public SmsTemplate GetSmsTemplateById(int id)
        {
            //string connectionString = "data source=162.241.138.178;Initial Catalog=Subzz_Settings;user id=subzz_user;password=w9if%l10;multipleactiveresultsets=true";
            string connectionString = "Data Source=DESKTOP-V00GN3I\\SQLEXPRESS;Initial Catalog=Subzz_Settings;Integrated Security=True";
            //string connectionString = "data source=162.241.138.178\\stg;Initial Catalog=Subzz_Settings;user id=tamoor;password=password;multipleactiveresultsets=true";
            List<SmsTemplate> smsTemplatesList;

            using (var dc = new SqlConnection(connectionString))
            {
                smsTemplatesList = dc.Query<SmsTemplate>(@"
					SELECT
						[SmsTemplate_Id] as Id				
						,[TextContent]
						,[SendTo]					
					FROM [Subzz_Settings].[Settings].[SmsTemplate]
					WHERE [SmsTemplate_Id] = " + id + "", id).ToList();
            }
            if (!smsTemplatesList.Any())
            {
                throw new ArgumentException(string.Format("Template with specified id {0} doesn't exist", id), "id");
            }
            return smsTemplatesList.FirstOrDefault();
        }
        public async Task<MailTemplate> GetMailTemplateByIdAsync(int id)
        {
            List<MailTemplate> mailTemplatesList;
            using (var dc = new SqlConnection("Data Source=DESKTOP-V00GN3I\\SQLEXPRESS;Initial Catalog=Subzz_Settings;Integrated Security=True"))
            //using (var dc = new SqlConnection("data source=162.241.138.178;Initial Catalog=Subzz_Settings;user id=subzz_user;password=w9if%l10;multipleactiveresultsets=true"))
            //using (var dc = new SqlConnection("data source=162.241.138.178\\stg;Initial Catalog=Subzz_Settings;user id=tamoor;password=password;multipleactiveresultsets=true"))
            {
                try
                {
                    mailTemplatesList = dc.QueryAsync<MailTemplate>(@"
					SELECT
						[EmailTemplate_Id]
						,[Title]
						,[EmailContent]
						,[EmailDisclaimerNeeded]
						,[Notes]
						,[SendTo]
						,[SenderEmail]
						,[EmailDisclaimerContent]
						,[SupportEmail]
					FROM [Settings].[EmailTemplate]
					WHERE [EmailTemplate_Id] = " + id + "").Result.ToList();
                    if (!mailTemplatesList.Any())
                    {
                        throw new ArgumentException(string.Format("Template with specified id {0} doesn't exist", id), "id");
                    }

                    return mailTemplatesList.FirstOrDefault();
                }
                catch (Exception ex)
                {

                }

            }
            return null;
        }
    }
}