using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Manage.Interface
{
    public interface IDistrictRepository
    {
        DistrictModel InsertDistrict(DistrictModel model);
        DistrictModel UpdateDistrict(DistrictModel model);
        IEnumerable<DistrictModel> GetDistricts();
        bool DeleteDistrict(int id);
        IEnumerable<DistrictModel> GetDistrict(int id);
    }
}
