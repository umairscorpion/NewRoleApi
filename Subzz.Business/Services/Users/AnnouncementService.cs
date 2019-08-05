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
        public string InsertAnnouncement(OrganizationModel model)
        {
            return _repo.InsertAnnouncement(model);
        }

    }
}
