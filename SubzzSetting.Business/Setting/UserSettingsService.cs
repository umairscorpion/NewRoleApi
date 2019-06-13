using SubzzSetting.Business.Setting.Interface;
using SubzzSettings.DataAccess.Settings.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzSetting.Business.Setting
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _repo;
        public UserSettingsService(IUserSettingsRepository repo)
        {
            _repo = repo;
        }

        public NoticationSettingsModel GetNotificationSettings(string UserId)
        {
            return _repo.GetNotificationSettings(UserId);
        }

        public SubzzVersion GetLatestVersionDetails()
        {
            return _repo.GetLatestVersionDetails();
        }
    }
}
