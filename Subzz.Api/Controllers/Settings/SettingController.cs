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

        [Route("version")]
        [HttpGet]
        public IActionResult GetLatestVersionUpdate()
        {
            var userId = base.CurrentUser.Id;
            var versionUpdates = _service.GetLatestVersionDetails();
            return Ok(versionUpdates);
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
        public PayRateSettings InsertPayRate([FromBody] PayRateSettings payRateSettings)
        {
            var Settings = _userService.InsertPayRate(payRateSettings);
            return Settings;
        }

        [Route("payRate")]
        [HttpPatch]
        public IActionResult UpdatePayRate([FromBody]PayRateSettings payRateSettings)
        {
            var positions = _userService.InsertPayRate(payRateSettings);
            return Ok(positions);
        }

        [Route("getPayRate/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRates(int districtId)
        {
            var positions = _userService.GetPayRates(districtId);
            return Ok(positions);
        }

        [Route("deletePayRate{id}")]
        [HttpDelete]
        public IActionResult DeletePayRate(int id)
        {
            var payRate = new PayRateSettings();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _userService.DeletePayRate(payRate);
            return Ok(result);
        }

        [Route("payRateRule")]
        [HttpPost]
        public IActionResult InsertPayRateRule([FromBody]PayRateRule payRateRule)
        {
            var rule = _userService.InsertPayRateRule(payRateRule);
            return Ok(rule);
        }

        [Route("payRateRule")]
        [HttpPatch]
        public IActionResult UpdatePayRateRule([FromBody]PayRateRule payRateRule)
        {
            var rule = _userService.InsertPayRateRule(payRateRule);
            return Ok(rule);
        }

        [Route("getPayRateRule/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRateRules(int districtId)
        {
            var positions = _userService.GetPayRateRules(districtId);
            return Ok(positions);
        }

        [Route("deletePayRateRule{id}")]
        [HttpDelete]
        public IActionResult DeletePayRateRule(int id)
        {
            var payRate = new PayRateRule();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _userService.DeletePayRateRule(payRate);
            return Ok(result);
        }
        #endregion
    }
}