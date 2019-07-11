﻿using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzAbsence.Business.Absence.Interface
{
    public interface IAbsenceService
    {
        AbsenceModel CreateAbsence(AbsenceModel model);
        Task<int> SaveAsSingleDayAbsence(DataTable absences);
        IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId, string CampusId);
        IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId);
        int UpdateAbsenceStatus(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId);
        IEnumerable<AbsenceModel> GetAbsencesByStatus(int StatusId);
        AbsenceModel GetAbsenceDetailByAbsenceId(int AbsenceId);
        Task<int> CreatePreferredAbsenceHistory(IEnumerable<User> Substitutes, AbsenceModel absences);
        IEnumerable<PreferredSubstituteModel> GetFavSubsForSendingSms(DateTime date);
        List<PreferredSubstituteModel> GetFavSubsForSendingSmsAndEmail(DateTime date);
        int UpdateAbsenceStatusAndSub(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired);
        int UpdateAbsence(AbsenceModel model);
        //List<AbsenceSummary> GetAbsenceSummary(string userId, int year);
        DashboardSummary GetAbsenceSummary(string userId, int year);
        List<AbsenceSummary> GetTopTenTeachers(string userId);
        List<Event> GetEvents(DateTime startDate, DateTime endDate, string userId);
        void UpdateMailAndSmsFlag(int id, bool IsSendSms, bool IsSendEmail);
        void UpdateNotificationflagForAll(int absenceId);
        int GetAbsenceIdByConfirmationNumber(string ConfirmationNumber);
    }
}
