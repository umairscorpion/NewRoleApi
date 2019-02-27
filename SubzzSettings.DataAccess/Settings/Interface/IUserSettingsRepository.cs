using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzSettings.DataAccess.Settings.Interface
{
    public interface IUserSettingsRepository
    {
        NoticationSettingsModel GetNotificationSettings( string UserId);
    }
}
