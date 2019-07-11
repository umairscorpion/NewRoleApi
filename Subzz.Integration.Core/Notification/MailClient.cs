using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net.Mime;
using System.Threading.Tasks;
using SubzzV2.Integration.Core.Notification.Interface;
using Subzz.Integration.Core.Domain;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using System.Reflection;
using System.Net.Http;

namespace SubzzV2.Integration.Core.Notification
{
    public class MailClient : IEmailClient
    {
        //TODO: inject LicenseKey value from appSettings by adding static property
        //static MailClient()
        //{
        //    Smtp.LicenseKey = "MN600-3BF3CCAEF3F6F3A1F33E6353E3F9-717E";
        //}

        private readonly MailSettings _mailSettings;
        public MailClient(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }


        public void Send(EmailMessage emailMessage)
        {
            //MailMessage mailMessage = CreateMaillBeeMessage(emailMessage);
            System.Net.Mail.MailMessage mailMessage = CreateMaillMessage(emailMessage);
            //try
            //{
            //    Execute(mailMessage);
            //}
            //catch (MailBeeLoginNoCredentialsException e)
            //{
            //    _logger.LogError(e, SmtpStatusCode.ClientNotPermitted.ToString(), "MailClient");
            //    throw new SmtpException(SmtpStatusCode.ClientNotPermitted, MailClientExceptionDescription.MailBeeLoginNoCredentials);
            //}
            //catch (MailBeeSmtpNoAcceptedRecipientsException e)
            //{
            //    _logger.LogError(e, MailClientExceptionDescription.NoAcceptedRecipientsError.ToString(), "MailClient");
            //    throw new SmtpException(MailClientExceptionDescription.NoAcceptedRecipientsError, e.InnerException);
            //}
            //catch (MailBeeSmtpLoginBadCredentialsException e)
            //{
            //    _logger.LogError(e, MailClientExceptionDescription.MailBeeSmtpLoginBadCredentials.ToString(), "MailClient");
            //    throw new SmtpException(MailClientExceptionDescription.MailBeeSmtpLoginBadCredentials, e.InnerException);
            //}
            //catch (MailBeeSocketTimeoutException e)
            //{
            //    _logger.LogError(e, MailClientExceptionDescription.MailBeeSocketTimeout.ToString(), "MailClient");
            //    throw new SmtpException(MailClientExceptionDescription.MailBeeSocketTimeout, e.InnerException);
            //}
            //catch (System.Exception e)
            //{
            //    _logger.LogError(e, "Send", "MailClient");
            //    throw new SmtpException("ErrorOnSend", e.InnerException);
            //}
        }

        public void Send(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string base64, List<MailsAttachment> mailAttachments = null,
         string charset = "utf-8")
        {
            from = HttpUtility.HtmlDecode(from);
            //MailPriority mailPriority = MailPriority.None;
            var toEmails = new List<string>();
            if (to == null)
            {
                //throw new SmtpException(MailClientExceptionDescription.NoRecepientError);
            }
            if (to.Length != 0)
            {
                foreach (string r in to)
                {
                    if (r.Trim() != String.Empty)
                    {
                        toEmails.Add(r);
                    }
                }
            }
            if (toEmails.Count == 0)
            {
                //throw new SmtpException(MailClientExceptionDescription.NoRecepientError);
            }
            try
            {
                //var message = new MailMessage
                //{
                //    Subject = subject,
                //    Charset = charset,
                //    Priority = mailPriority,
                //    From = new EmailAddress
                //    {
                //        AsString = from
                //    }
                //};

                var message = new System.Net.Mail.MailMessage();
                message.Subject = subject;
                message.Priority = System.Net.Mail.MailPriority.Normal;
                message.From = new MailAddress(from);
                //if (isHtmlBody)
                //{
                message.Body = HttpUtility.HtmlDecode(htmlBody);
                message.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                message.IsBodyHtml = true;

                if (mailAttachments != null && mailAttachments.Count > 0)
                {
                    foreach (var file in mailAttachments)
                    {
                        file.FileStream.Seek(0, SeekOrigin.Begin);
                        //message.Attachments.Add(new System.Net.Mail.Attachment(file.FileStream, file.FileNameWithExtension, MimeMapping.GetMimeMapping(file.FileNameWithExtension)));
                    }                    
                }
                //}
                /*else {
					message.BodyPlainText = htmlBody;
					message.MailTransferEncodingPlain = MailTransferEncoding.QuotedPrintable;
				}*/

                //foreach (var item in toEmails)
                //{
                //    if (!String.IsNullOrEmpty(item.Trim()))
                //    {
                //        message.To.Add(new EmailAddress
                //        {
                //            AsString = item.Trim()
                //        });
                //        message.To.Add(new string(item.Trim()));
                //    }
                //}

                foreach (var toAddress in toEmails)
                {
                    if (!String.IsNullOrEmpty(toAddress.Trim()))
                        message.To.Add(toAddress);
                };

                if (!string.IsNullOrEmpty(base64) && message.Body.IndexOf("cid:image1") != -1)
                {
                    byte[] data = System.Convert.FromBase64String(base64);
                    MemoryStream ms = new MemoryStream(data);

                    AlternateView av = AlternateView.CreateAlternateViewFromString(htmlBody,
                                null, MediaTypeNames.Text.Html);

                    LinkedResource lr = new LinkedResource(ms, MediaTypeNames.Image.Jpeg);
                    lr.ContentId = "image1";
                    av.LinkedResources.Add(lr);
                    message.AlternateViews.Add(av);
                }

                //Execute(message);
            }
            catch (FormatException e)
            {
                throw new SmtpException("ErrorOnSend", e);
            }
        }

        public async Task SendAsync(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string base64, string charset = "utf-8")
        {
            try
            {
                // Construct an object to contain the recipient address.
                Destination destination = new Destination();
                destination.ToAddresses = (new List<string>() { to.FirstOrDefault() });
                // Create the subject and body of the message.
                Content emailSubject = new Content(subject);
                Content textBody = new Content(htmlBody);
                Body body = new Body();
                body.Html = textBody;
                Amazon.SimpleEmail.Model.Message message = new Amazon.SimpleEmail.Model.Message(emailSubject, body);
                SendEmailRequest request = new SendEmailRequest(from, destination, message);
                await ExecuteAsync(request);
            }
            catch (FormatException e)
            {
                throw new SmtpException("ErrorOnSend", e);
            }
        }

        public Task ExecuteAsync(SendEmailRequest request)
        {
            try
            {

                Amazon.RegionEndpoint regionEndPoint = Amazon.RegionEndpoint.USWest2;
                AmazonSimpleEmailServiceConfig config = new AmazonSimpleEmailServiceConfig();
                config.RegionEndpoint = regionEndPoint;
                AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_mailSettings.AwsAccessKeyId, _mailSettings.AwsSecretAccessKey, config);
                return  client.SendEmailAsync(request);
            }
            catch (Exception ex)
            {

            }

            return null;
        }

      
        //private async Task ExecuteAsync(System.Net.Mail.MailMessage mailMessage)
        //{
        //    try
        //    {
        //        using (var client = CreateSmtp())
        //        {
        //            client.Send(mailMessage);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        _logger.LogError(ex, "Execute method.", "MailClient");
        //        //Execute(mailMessage);
        //    }
        //}

        private void Execute(MailMessage mailMessage)
        {
            try
            {
                //using (var client = CreateSmtpClient())
                //{
                //    client.Message = mailMessage;
                //    client.Send();
                //}
            }
            catch (System.Exception ex)
            {
                TryToSendOnError(mailMessage);
            }
        }

        private void TryToSendOnError(MailMessage mailMessage)
        {
            try
            {
                //using (var client = CreateSmtpClient())
                //{
                //    client.Message = mailMessage;
                //    client.Send();
                //}
            }
            catch (System.Exception ex)
            {
            }
        }

        //private Smtp CreateSmtpClient()
        //{
        //    var client = new Smtp();
        //    var server = new SmtpServer(_mailSettings.SmtpServer)
        //    {
        //        AccountName = _mailSettings.SmtpUserName,
        //        Password = _mailSettings.SmtpPassword,
        //        Port = _mailSettings.SmtpPort,
        //        SslMode = _mailSettings.EnableSsl ? SslStartupMode.UseStartTlsIfSupported : SslStartupMode.Manual,
        //        Timeout = _mailSettings.Timeout * 10,
        //        SmtpOptions = ExtendedSmtpOptions.NoChunking
        //    };
        //    client.SmtpServers.Add(server);
        //    //server.AuthMethods = GetAuthenticationMethods(client);
        //    return client;
        //}

        private SmtpClient CreateSmtp()
        {
            SmtpClient client = new SmtpClient();
            //client.Credentials = new System.Net.NetworkCredential(_mailSettings.SmtpUserName, _mailSettings.SmtpPassword);
            //client.Port = _mailSettings.SmtpPort;
            //client.Host = _mailSettings.SmtpServer;
            //client.EnableSsl = _mailSettings.EnableSsl;
            ////client.Timeout = _mailSettings.Timeout * 10;
            int timeout = client.Timeout;         
            return client;
        }

        //private AuthenticationMethods GetAuthenticationMethods(Smtp client)
        //{
        //    if (client == null)
        //    {
        //        string errMessage = "Smtp client should be initialized.";
        //        throw new NullReferenceException(errMessage);
        //    }
        //    if (_mailSettings.IsAnonymousAuthentication)
        //    {
        //        return AuthenticationMethods.None;
        //    }

        //    try
        //    {
        //        client.Connect();
        //        client.Hello();
        //        //AuthenticationMethods methods = client.GetSupportedAuthMethods();
        //        client.Disconnect();
        //        return methods;
        //    }
        //    catch (System.Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        private MailMessage CreateMaillBeeMessage(EmailMessage emailMessage)
        {
            //EmailAddressCollection to = GetEmailAddressCollection(emailMessage.To);
            //EmailAddressCollection cc = GetEmailAddressCollection(emailMessage.Cc);
            //EmailAddressCollection bcc = GetEmailAddressCollection(emailMessage.Bcc);
            //var mailMessage = new MailMessage
            //{
            //    Subject = emailMessage.Subject,
            //    Charset = "utf-8",
            //    Priority = MailPriority.None,
            //    From = new EmailAddress
            //    {
            //        AsString = emailMessage.From.FirstOrDefault()
            //    },
            //    To = to,
            //    Cc = cc,
            //    Bcc = bcc,
            //    BodyHtmlText = emailMessage.Body,
            //    MailTransferEncodingHtml = MailTransferEncoding.QuotedPrintable
            //};
            //mailMessage = SetContentTypeHeader(mailMessage);
            return null;
        }

        private System.Net.Mail.MailMessage CreateMaillMessage(EmailMessage emailMessage)
        {
            //MailAddressCollection to = GetMailAddressCollection(emailMessage.To);
            //MailAddressCollection cc = GetMailAddressCollection(emailMessage.Cc);
            //MailAddressCollection bcc = GetMailAddressCollection(emailMessage.Bcc);

            if (emailMessage.To == null)
                emailMessage.To = new List<string>();


            if (emailMessage.Cc == null)
                emailMessage.Cc = new List<string>();

            if (emailMessage.Bcc == null)
                emailMessage.Bcc = new List<string>();

            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.Subject = emailMessage.Subject;
            mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
            mailMessage.From = new MailAddress(emailMessage.From.FirstOrDefault());

            foreach (var toAddress in emailMessage.To)
            {
                mailMessage.To.Add(toAddress);
            };

            foreach (var ccAddress in emailMessage.Cc)
            {
                mailMessage.CC.Add(ccAddress);
            };

            foreach (var bccAddress in emailMessage.Cc)
            {
                mailMessage.Bcc.Add(bccAddress);
            };
            mailMessage.Body = emailMessage.Body;
            mailMessage.IsBodyHtml = true;           
            mailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            //var mailMessage = new MailMessage
            //{
            //    Subject = emailMessage.Subject,
            //    Charset = "utf-8",
            //    Priority = MailPriority.None,
            //    From = new EmailAddress
            //    {
            //        AsString = emailMessage.From.FirstOrDefault()
            //    },
            //    To = to,
            //    Cc = cc,
            //    Bcc = bcc,
            //    BodyHtmlText = emailMessage.Body,
            //    MailTransferEncodingHtml = MailTransferEncoding.QuotedPrintable
            //};
            //mailMessage = SetContentTypeHeader(mailMessage);
            return mailMessage;
        }
        private MailMessage SetContentTypeHeader(MailMessage message)
        {
            //MemoryStream stream = new MemoryStream();
            //message.SaveMessage(stream);
            //stream.Position = 0;
            //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            //{
            //    string value = reader.ReadToEnd();
            //    value = value.Replace("multipart/related", "multipart/mixed");
            //    byte[] byteArray = Encoding.UTF8.GetBytes(value);
            //    stream = new MemoryStream(byteArray);
            //    message.LoadMessage(stream);
            //}
            return message;
        }

        //private static EmailAddressCollection GetEmailAddressCollection(IEnumerable<string> emailAddresses)
        //{
        //    var emailAddressCollection = new EmailAddressCollection();
        //    if (emailAddresses == null)
        //    {
        //        return emailAddressCollection;
        //    }
        //    foreach (var emailAddress in emailAddresses)
        //    {
        //        emailAddressCollection.Add(emailAddress);
        //    }
        //    return emailAddressCollection;
        //}

        private static MailAddressCollection GetMailAddressCollection(IEnumerable<string> emailAddresses)
        {
            var emailAddressCollection = new MailAddressCollection();
            if (emailAddresses == null)
            {
                return emailAddressCollection;
            }
            foreach (var emailAddress in emailAddresses)
            {
                emailAddressCollection.Add(emailAddress);
            }
            return emailAddressCollection;
        }

        public Task SendRawEmail(string htmlBody, string subject, string[] to, string from, bool isHtmlBody, string attachedFileName, string fileContent)
        {
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(new MailAddress( to.FirstOrDefault() ));

            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            if (htmlBody != null)
            {
                mailMessage.AlternateViews.Add(htmlView);
            }

            if (attachedFileName.Trim() != "")
            {
                string folderName = "Attachment";
                string attachPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), folderName);
                if (System.IO.File.Exists(Path.Combine(attachPath, attachedFileName)))
                {
                    mailMessage.Attachments.Add(new Attachment(attachPath));
                    System.Net.Mail.Attachment objAttach = new System.Net.Mail.Attachment(attachPath);
                    objAttach.ContentType = new ContentType(fileContent);
                    System.Net.Mime.ContentDisposition disposition = objAttach.ContentDisposition;
                    disposition.DispositionType = "attachment";
                    disposition.CreationDate = System.IO.File.GetCreationTime(attachPath);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachPath);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(attachPath);
                    
                }
            }
            RawMessage rawMessage = new RawMessage(ConvertMailMessageToMemoryStream(mailMessage));
            SendRawEmailRequest request = new SendRawEmailRequest(rawMessage);
            Amazon.RegionEndpoint regionEndPoint = Amazon.RegionEndpoint.USWest2;
            AmazonSimpleEmailServiceConfig config = new AmazonSimpleEmailServiceConfig();
            config.RegionEndpoint = regionEndPoint;
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_mailSettings.AwsAccessKeyId, _mailSettings.AwsSecretAccessKey, config);
            return client.SendRawEmailAsync(request);
        }

        public static MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
        {
            Assembly assembly = typeof(SmtpClient).Assembly;
            Type mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
            using (MemoryStream stream = new MemoryStream())
            {
                ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);
                object mailWriter = mailWriterContructor.Invoke(new object[] { stream });
                MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);
                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);
                MethodInfo closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
                return stream;
            }
        }
    }
}
