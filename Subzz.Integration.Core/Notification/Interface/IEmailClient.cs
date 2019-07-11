using Subzz.Integration.Core.Domain;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification.Interface
{
    /// <summary>
    ///Generic interface that provide options to send e-mail message 
    /// </summary>
    public interface IEmailClient
    {
        void Send(EmailMessage emailMessage);

        void Send(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string base64, List<MailsAttachment> mailAttachments = null, string charset = "utf-8");

        Task SendAsync(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string base64, string charset = "utf-8");
        Task SendRawEmail(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string attachedFileName, string fileContentType);
    }
}