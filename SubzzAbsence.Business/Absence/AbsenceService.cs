﻿using Microsoft.Extensions.Configuration;
using SubzzAbsence.Business.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Absence;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzAbsence.Business.Absence
{
    public class AbsenceService : IAbsenceService
    {
        private readonly IAbsenceRepository _repo;
        public AbsenceService()
        {
            _repo = new AbsenceRepository();
        }
        public AbsenceService(IAbsenceRepository repo)
        {
            _repo = repo;
        }

        public int CreateAbsence(AbsenceModel model)
        {
            var absenceCreation = _repo.CreateAbsence(model);
            return absenceCreation;
        }

        public async Task<int> SaveAsSingleDayAbsence(DataTable absences)
        {
            return await _repo.SaveAsSingleDayAbsence(absences);
        }

        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId)
        {
             return _repo.GetAbsences(StartDate, EndDate, UserId);
        }

        public IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId)
        {
            return _repo.GetAbsencesScheduleEmployee(StartDate, EndDate, UserId);
        }

        public IEnumerable<AbsenceModel> GetAbsencesByStatus(int StatusId)
        {
            return _repo.GetAbsencesByStatus(StatusId);
        }

        public int UpdateAbsenceStatus(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId)
        {
            return _repo.UpdateAbsenceStatus(AbsenceId, statusId, UpdateStatusDate, UserId);
        }

        public AbsenceModel GetAbsenceDetailByAbsenceId(int AbsenceId)
        {
            return _repo.GetAbsenceDetailByAbsenceId(AbsenceId);
        }

        public async Task<int> CreatePreferredAbsenceHistory(IEnumerable<User> Substitutes, AbsenceModel absence)
        {
            return await _repo.CreatePreferredAbsenceHistory(Substitutes, absence);
        }

        public IEnumerable<PreferredSubstituteModel> GetFavSubsForSendingSms(DateTime date)
        {
            return _repo.GetFavSubsForSendingSms(date);
        }
    }
}