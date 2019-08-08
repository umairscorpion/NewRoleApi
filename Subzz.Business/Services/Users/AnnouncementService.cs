using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Users.Interface;
using System.Collections.Generic;

namespace Subzz.Business.Services.Users
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _repo;
        public AnnouncementService(IAnnouncementRepository repo)
        {
            _repo = repo;
        }

        public string InsertAnnouncement(Announcements model)
        {
            return _repo.InsertAnnouncement(model);
        }

        public List<Announcements> GetAnnouncements(Announcements model)
        {
            return _repo.GetAnnouncements(model);
        }
    }
}
