﻿using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.District.Interface
{
    public interface IDistrictService
    {
        DistrictModel InsertDistrict(DistrictModel model);
        DistrictModel UpdateDistrict(DistrictModel model);
        IEnumerable<DistrictModel> GetDistricts();
        bool DeleteDistrict(int id);
        IEnumerable<DistrictModel> GetDistrict(int id);
    }

}
