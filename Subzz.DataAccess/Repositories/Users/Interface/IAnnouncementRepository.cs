using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.DataAccess.Repositories.Users.Interface
{
    public interface IAnnouncementRepository
    {
        string InsertAnnouncement(Announcements model);
        List<Announcements> GetAnnouncements(Announcements model);
    }
}
