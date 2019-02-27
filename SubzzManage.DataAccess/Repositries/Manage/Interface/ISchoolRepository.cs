using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Manage.Interface
{
    public interface ISchoolRepository
    {
        OrganizationModel InsertSchool(OrganizationModel user);
        OrganizationModel UpdateSchool(OrganizationModel model);
        IEnumerable<OrganizationModel> GetSchools();
        bool DeleteSchool(string schoolId);
        IEnumerable<OrganizationModel> GetSchool(string schoolId);
        IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int districtId);
        LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId);    
    }
}
