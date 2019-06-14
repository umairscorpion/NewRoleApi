using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
using System;

namespace Subzz.Api.Controllers.User
{
    [Route("api/event")]
    public class EventController : BaseApiController
    {
        private readonly IUserService _service;

        public EventController(IUserService service)
        {
            _service = service;
        }

        [Route("")]
        [HttpPost]
        public IActionResult Post([FromBody]Event model)
        {
            try
            {
            model.UserId = base.CurrentUser.Id;
            model.CreatedBy = base.CurrentUser.Id;
            var result = _service.InsertEvent(model);
            return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
    }
}