using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;

namespace Subzz.Api.Controllers.Communication
{
    [Produces("application/json")]
    [Route("api/Communication")]
    public class CommunicationController : Controller
    {
        private readonly IUserService _service;
        public CommunicationController(IUserService service)
        {
            _service = service;
        }
        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }
        [Route("sendWellcomeLetter")]
        [HttpPost]
        public IActionResult SendWellcomeLetter(SubzzV2.Core.Entities.User user)
        {
            Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
            message.Email = user.Email;
            message.EmployeeName = user.FirstName;
            message.PhoneNumber = user.PhoneNumber;
            //CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
            return Ok();
        }
    }
}