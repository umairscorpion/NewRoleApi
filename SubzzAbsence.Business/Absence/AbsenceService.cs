using Microsoft.Extensions.Configuration;
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

        public AbsenceModel CreateAbsence(AbsenceModel model)
        {
            var absenceCreation = _repo.CreateAbsence(model);
            return absenceCreation;
        }

        public async Task<int> SaveAsSingleDayAbsence(DataTable absences)
        {
            return await _repo.SaveAsSingleDayAbsence(absences);
        }

        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId, string CampusId)
        {
             return _repo.GetAbsences(StartDate, EndDate, UserId, CampusId);
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

        public int CheckNegativeAllowance(int AllowanceType, string UserId, string EndDate, string StartDate)
        {
            return _repo.CheckNegativeAllowance(AllowanceType, UserId, EndDate, StartDate);
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

        public List<PreferredSubstituteModel> GetFavSubsForSendingSmsAndEmail(DateTime date)
        {
            return _repo.GetFavSubsForSendingSmsAndEmail(date);
        }

        public string UpdateAbsence(AbsenceModel user)
        {
            return _repo.UpdateAbsence(user);
        }

        public int UpdateAbsenceStatusAndSub(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired)
        {
            return _repo.UpdateAbsenceStatusAndSub(AbsenceId, statusId, UpdateStatusDate, UserId, SubstituteId, SubstituteRequired);
        }

        //public List<AbsenceSummary> GetAbsenceSummary(string userId, int year)
        //{
        //    return _repo.GetAbsenceSummary(userId, year);
        //}

        public DashboardSummary GetAbsenceSummary(string userId, int year)
        {
            return _repo.GetAbsenceSummary(userId, year);
        }

        public List<AbsenceSummary> GetTopTenTeachers(string userId)
        {
            return _repo.GetTopTenTeachers(userId);
        }

        public List<Event> GetEvents(DateTime startDate, DateTime endDate, string userId)
        {
            return _repo.GetEvents(startDate, endDate, userId);
        }

        public void UpdateMailAndSmsFlag(int id, bool IsSendSms, bool IsSendEmail)
        {
              _repo.UpdateMailAndSmsFlag(id, IsSendSms, IsSendEmail);
        }

        public void UpdateNotificationflagForAll(int absenceId)
        {
            _repo.UpdateNotificationflagForAll(absenceId);
        }

        public int GetAbsenceIdByConfirmationNumber(string ConfirmationNumber)
        {
            return _repo.GetAbsenceIdByConfirmationNumber(ConfirmationNumber);
        }

        public IEnumerable<AbsenceModel> GetAbsencesForSharedCalendar(AbsenceModel model)
        {
            return _repo.GetAbsencesForSharedCalendar(model);
        }

        public async Task<IEnumerable<AbsenceModel>> GetAbsencesForCalendar(DateTime StartDate, DateTime EndDate, string UserId)
        {
            return await _repo.GetAbsencesForCalendar(StartDate, EndDate, UserId);
        }

        public int UpdateAbsenceResendCounter(int AbsenceId)
        {
            return _repo.UpdateAbsenceResendCounter(AbsenceId);
        }
    }
}
