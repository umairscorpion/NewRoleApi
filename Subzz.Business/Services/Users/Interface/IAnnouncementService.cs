using SubzzV2.Core.Models;
using System.Collections.Generic;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IAnnouncementService
    {
        string InsertAnnouncement(Announcements model);
        List<Announcements> GetAnnouncements(Announcements model);
    }
}
