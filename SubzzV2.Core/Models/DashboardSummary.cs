using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class DashboardSummary
    {
        public DashboardSummary()
        {
            DashboardTopCounters = new List<DashboardTopCounters>();
            AbsenceSummaryTotal = new List<AbsenceSummaryTotal>();
            TopFourAbsenceReasons = new List<TopFourAbsenceReasons>();
            FilledUnfilledTenDay = new List<FilledUnfilledTenDay>();
            TopTenTeachers = new List<TopTenTeachers>();
            FillRate = new List<FillRate>();
            AbsenceBySubject = new List<AbsenceBySubject>();
            AbsenceByWeek = new List<AbsenceByWeek>();
            AbsenceByGradeLevel = new List<AbsenceByGradeLevel>();
            AbsenceSummary = new List<AbsenceSummary>();
        }
        public List<DashboardTopCounters> DashboardTopCounters { get; set; }
        public List<AbsenceSummaryTotal> AbsenceSummaryTotal { get; set; }
        public List<TopFourAbsenceReasons> TopFourAbsenceReasons { get; set; }
        public List<FilledUnfilledTenDay> FilledUnfilledTenDay { get; set; }
        public List<FillRate> FillRate { get; set; }
        public List<AbsenceBySubject> AbsenceBySubject { get; set; }
        public List<AbsenceByWeek> AbsenceByWeek { get; set; }
        public List<AbsenceByGradeLevel> AbsenceByGradeLevel { get; set; }
        public List<TopTenTeachers> TopTenTeachers { get; set; }
        public List<AbsenceSummary> AbsenceSummary { get; set; }
    }
}
