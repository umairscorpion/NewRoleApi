using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzSetting.Business.Setting.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Settings
{
    [Produces("application/json")]
    [Route("api/Setting")]
    public class SettingController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IUserSettingsService _service;
        public SettingController(IUserSettingsService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }
        [Route("getNotificationSettings")]
        [HttpGet]
        public NoticationSettingsModel GetNotificationSettings()
        {
            var userId = base.CurrentUser.Id;
            var Settings = _service.GetNotificationSettings(userId);
            return Settings;
        }

        #region PayRateSetting
        [Route("payRate")]
        [HttpPost]
        public NoticationSettingsModel InsertPayRate()
        {
            var userId = base.CurrentUser.Id;
            var Settings = _service.GetNotificationSettings(userId);
            return Settings;
        }
        #endregion
    }
}