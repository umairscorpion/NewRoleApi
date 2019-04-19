using SubzzV2.Core.Entities;
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
        int CreateAbsence(AbsenceModel model);
        Task<int> SaveAsSingleDayAbsence(DataTable absences);
        IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId);
        IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId);
        int UpdateAbsenceStatus(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId);
        IEnumerable<AbsenceModel> GetAbsencesByStatus(int StatusId);
        AbsenceModel GetAbsenceDetailByAbsenceId(int AbsenceId);
        Task<int> CreatePreferredAbsenceHistory(IEnumerable<User> Substitutes, AbsenceModel absences);
        IEnumerable<PreferredSubstituteModel> GetFavSubsForSendingSms(DateTime date);
        int UpdateAbsenceStatusAndSub(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired);
        int UpdateAbsence(AbsenceModel model);
    }
}
