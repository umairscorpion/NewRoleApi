﻿using SubzzManage.Business.District.Interface;
using SubzzManage.DataAccess.Repositries.Manage.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.District
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _repo;
        public DistrictService(IDistrictRepository repo)
        {
            _repo = repo;
        }
        public DistrictModel InsertDistrict(DistrictModel model)
        {
            return _repo.InsertDistrict(model);
        }
        public DistrictModel UpdateDistrict(DistrictModel model)
        {
            return _repo.UpdateDistrict(model);
        }
        public IEnumerable<DistrictModel> GetDistricts()
        {
            return _repo.GetDistricts();
        }
        public int DeleteDistrict(int id)
        {
            return _repo.DeleteDistrict(id);
        }
        public IEnumerable<DistrictModel> GetDistrict(int id)
        {
            return _repo.GetDistrict(id);
        }

        public DistrictModel UpdateSettings(DistrictModel model)
        {
            return _repo.UpdateSettings(model);
        }

        public Allowance AddAllowance(Allowance model)
        {
            return _repo.AddAllowance(model);
        }

        public IEnumerable<Allowance> GetAllowances(string districtId)
        {
            return _repo.GetAllowances(districtId);
        }

        public int DeleteAllowance(int id)
        {
            return _repo.DeleteAllowance(id);
        }
    }
}
