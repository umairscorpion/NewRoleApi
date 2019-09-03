using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.Manage.Interface
{
    public interface ISchoolService
    {
        OrganizationModel InsertSchool(OrganizationModel user);
        OrganizationModel UpdateSchool(OrganizationModel model);
        IEnumerable<OrganizationModel> GetSchools();
        int DeleteSchool(string schoolId);
        IEnumerable<OrganizationModel> GetSchool(string schoolId);
        IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int districtId);
        LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId);
        IEnumerable<AbsenceScope> GetAbsenceScopes(OrganizationModel organizationModel);
        AbsenceScope UpdateAbsenceScope(AbsenceScope absenceScope);
        int GetDistrictId(string name);
    }
}
