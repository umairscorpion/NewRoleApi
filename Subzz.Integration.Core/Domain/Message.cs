using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
    public class Message
    {
        public Message()
        {
            MailAttachments = new List<MailsAttachment>();
        }

        public string ActivationCode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ActivationLink { get; set; }
        public string SiteUrlLink { get; set; }
        public bool IsActivationEmail { get; set; }
        public string SendTo { get; set; }
        public string SendToUser { get; set; }
        public string UserName { get; set; }
        public string EmployeeName { get; set; }
        public string ApprovedBy { get; set; }
        public string Reason { get; set; }
        public string Position { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string Location { get; set; }
        public string School { get; set; }
        public string Notes { get; set; }
        public string Duration { get; set; }
        public string Email { get; set; }
        public int? ExpiringDates { get; set; }
        public int UserId { get; set; }
        public string MailTo { get; set; }
        public string MessageText { get; set; }
        public string Photo { get; set; }
        public int TemplateId { get; set; }
        public string ImageBase64 { get; set; }
        public List<MailsAttachment> MailAttachments { get; set; }
        public string AttachedFileName { get; set; }
        public string FileContentType { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastname { get; set; }
        public string TimeInterval { get; set; }
        public string Sendtype { get; set; }
        public string AdminMailType { get; set; }
        public string PhoneNumber { get; set; }
        public int AbsenceId { get; set; }
        public string SubstituteName { get; set; }
        public string AcceptUrl { get; set; }
        public string DeclineUrl { get; set; }
        public string Password { get; set; }
        public string resetPassUrl { get; set; }
        public string ProfilePicUrl { get; set; }
        public string VerifyUrl { get; set; }

        public string StartTimeSMS { get; set; }
        public string EndTimeSMS { get; set; }
        public string StartDateSMS { get; set; }
        public string EndDateSMS { get; set; }
        public string DateToDisplayInSMS { get; set; }
        public string ConfirmationNumber { get; set; }
        public string UnsubscriptionUrl { get; set; }
        public string FromPhoneNumber { get; set; }

        public string RunningLateMessage { get; set; }

    }

    public class MailsAttachment
    {
        public MemoryStream FileStream { get; set; }
        public string FileNameWithExtension { get; set; }
    }
}