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
            try
            {
                var userId = base.CurrentUser.Id;
                var versionUpdates = _service.GetLatestVersionDetails();
                return Ok(versionUpdates);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getNotificationSettings")]
        [HttpGet]
        public NoticationSettingsModel GetNotificationSettings()
        {
            try
            {
            var userId = base.CurrentUser.Id;
            var Settings = _service.GetNotificationSettings(userId);
            return Settings;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        #region PayRateSetting
        [Route("payRate")]
        [HttpPost]
        public PayRateSettings InsertPayRate([FromBody] PayRateSettings payRateSettings)
        {
            try
            {
            var Settings = _userService.InsertPayRate(payRateSettings);
            return Settings;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payRate")]
        [HttpPatch]
        public IActionResult UpdatePayRate([FromBody]PayRateSettings payRateSettings)
        {
            try
            {
            var positions = _userService.InsertPayRate(payRateSettings);
            return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getPayRate/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRates(int districtId)
        {
            try
            { 
            var positions = _userService.GetPayRates(districtId);
            return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deletePayRate{id}")]
        [HttpDelete]
        public IActionResult DeletePayRate(int id)
        {
            try
            {
            var payRate = new PayRateSettings();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _userService.DeletePayRate(payRate);
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

        [Route("payRateRule")]
        [HttpPost]
        public IActionResult InsertPayRateRule([FromBody]PayRateRule payRateRule)
        {
            try
            { 
            var rule = _userService.InsertPayRateRule(payRateRule);
            return Ok(rule);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payRateRule")]
        [HttpPatch]
        public IActionResult UpdatePayRateRule([FromBody]PayRateRule payRateRule)
        {
            try
            { 
            var rule = _userService.InsertPayRateRule(payRateRule);
            return Ok(rule);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getPayRateRule/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRateRules(int districtId)
        {
            try
            { 
            var positions = _userService.GetPayRateRules(districtId);
            return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deletePayRateRule{id}")]
        [HttpDelete]
        public IActionResult DeletePayRateRule(int id)
        {
            try
            { 
            var payRate = new PayRateRule();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _userService.DeletePayRateRule(payRate);
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
        #endregion
    }
}