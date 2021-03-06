﻿
using SubzzManage.Business.Manage.Interface;
using SubzzManage.DataAccess.Repositries.Manage.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.Manage
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _repo;
        public SchoolService(ISchoolRepository repo)
        {
            _repo = repo;
        }

        public OrganizationModel InsertSchool(OrganizationModel model)
        {
            return _repo.InsertSchool(model);
        }

        public OrganizationModel InsertSchoolTemporary(OrganizationModel model, int DistrictId, string Status)
        {
            return _repo.InsertSchoolTemporary(model, DistrictId, Status);
        }

        public OrganizationModel UpdateSchool(OrganizationModel model)
        {
            return _repo.UpdateSchool(model);
        }

        public IEnumerable<OrganizationModel> GetSchools()
        {
            return _repo.GetSchools();
        }

        public IEnumerable<OrganizationModel> GetTemporarySchools()
        {
            return _repo.GetTemporarySchools();
        }

        public int DeleteSchool(string schoolId)
        {
            return _repo.DeleteSchool(schoolId);
        }

        public int DeleteTemporarySchools(int DistrictId)
        {
            return _repo.DeleteTemporarySchools(DistrictId);
        }

        public IEnumerable<OrganizationModel> GetSchool(string schoolId)
        {
            return _repo.GetSchool(schoolId);
        }

        public IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int districtId)
        {
            return _repo.GetOrganizationsByDistrictId(districtId);
        }

        public LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId)
        {
            return _repo.GetOrganizationTimeByOrganizationId(OrganizationId);
        }

        public IEnumerable<AbsenceScope> GetAbsenceScopes(OrganizationModel organizationModel)
        {
            return _repo.GetAbsenceScopes(organizationModel);
        }

        public AbsenceScope UpdateAbsenceScope(AbsenceScope absenceScope)
        {
            return _repo.UpdateAbsenceScope(absenceScope);
        }

        public int GetDistrictId(string name)
        {
            return _repo.GetDistrictId(name);
        }

        public int GetCountryId(string name)
        {
            return _repo.GetCountryId(name);
        }

        public int GetStateId(string name)
        {
            return _repo.GetStateId(name);
        }

        public IEnumerable<CountryModel> GetCountries()
        {
            return _repo.GetCountries();
        }

        public IEnumerable<StateModel> GetStateByCountryId(int counrtyId)
        {
            return _repo.GetStateByCountryId(counrtyId);
        }
    }
}
