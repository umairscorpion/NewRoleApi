using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzSetting.Business.Setting.Interface
{
    public interface IUserSettingsService
    {
        NoticationSettingsModel GetNotificationSettings(string UserId);
        SubzzVersion GetLatestVersionDetails();
    }
}
