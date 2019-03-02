using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class ReportDetail
    {
        public int AbsenceId { get; set; }
        public string EmployeeName { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public DateTime PostedOn { get; set; }
        public int PostedById { get; set; } 
        public string PostedByName { get; set; }
        public string StatusId { get; set; }
        public string StatusDate { get; set; }
        public int FilledById { get; set; }
        public string FilledByName { get; set; }
    }
}
