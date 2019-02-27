using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Custom
{
    public sealed class Notification
    {
        public delegate string SendEmail(AbsenceModel absenceModel);
        public static string ProcessRequest(AbsenceModel absenceModel)
        {
            try
            {
                const String from = "no-reply@loginsubzz.com";
                // Construct an object to contain the recipient address.
                Destination destination = new Destination();
                destination.ToAddresses = (new List<string>() { "" });
                Content emailSubject = new Content("asdasd");
                Content textBody = new Content("asdsad");
                Body body = new Body();
                body.Html = textBody;
                // Create a message with the specified subject and body.
                Amazon.SimpleEmail.Model.Message message = new Amazon.SimpleEmail.Model.Message(emailSubject, body);
                SendEmailRequest request = new SendEmailRequest(from, destination, message);
                Amazon.RegionEndpoint regionEndPoint = Amazon.RegionEndpoint.USWest2;
                AmazonSimpleEmailServiceConfig config = new AmazonSimpleEmailServiceConfig();
                config.RegionEndpoint = regionEndPoint;
                var key = "AKIAISEURJBJ3YL4J6EA";
                var sec = "7iiVlPs51EEoZw5+TjlE+z/XdEy9WCa0rvF91u6O";
                // Instantiate an Amazon SES client, which will make the service call.
                AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(key, sec, config);
                client.SendEmailAsync(request);
            }

            catch (Exception ex)
            {
                //log into database.

                string exception = "Email Sending Failed Amazon SES";
                if (ex.StackTrace != null)
                {
                    exception = exception + "Main Stack Trace: " + ex.StackTrace;
                    exception = exception + " Main Message " + Convert.ToString(ex.Message);
                }
                if (ex.InnerException != null)
                {
                    exception = exception + "Inner Stack Trace: " + ex.StackTrace;
                    exception = exception + " Inner Message " + Convert.ToString(ex.Message);
                }


                
            }
            return "";
        }
    }
}
