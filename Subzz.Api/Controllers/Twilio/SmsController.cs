using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Microsoft.AspNetCore.Mvc;


namespace Subzz.Api.Controllers.Twilio
{
    [Route("api/[controller]")]
    public class SmsController : TwilioController
    {
        [HttpGet]
        public TwiMLResult Index()
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Message Send: " +
                                      "");

            return TwiML(messagingResponse);
        }
    }
}