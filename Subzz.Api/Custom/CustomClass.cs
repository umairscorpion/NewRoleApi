using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Custom
{
    public static class CustomClass
    {
        #region STOREABSENCEASSINGLEABSENCE
        /// <summary>
        /// AFTER CREATING ABSENCE THIS METHOD STORE ABSENCE BASIC INFO IN SEPERATE TABLE
        /// IT CONVERTS MULTI DAT ABSENCE AS SINGLE DAY AND INSERT IN TABLE
        /// </summary>
        /// <param name="ConfirmationNo">ACONFIRMATION NO OF ABSENCE</param>
        /// /// <param name="StartDate">START DATE OF ABSENCE</param>
        /// /// <param name="EndDate">END DATE OF ABSENCE</param>
        /// /// <param name="StartTime">STARTTIME OF ABSENCE</param>
        /// /// <param name="EndTime">END TIME OF ABSENCE</param>
        /// <returns>dataTable, CONTAINS MULTIDAY ABSENCE AS SINGLE DAY</returns>
        public static DataTable InsertAbsenceBasicDetailAsSingleDay(int AbsenceId, DateTime StartDate, DateTime EndDate, TimeSpan StartTime, TimeSpan EndTime)
        {
            DateTime startTime = DateTime.ParseExact(Convert.ToString(StartTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(Convert.ToString(EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var dataTable = new DataTable();
            dataTable.Columns.Add("AbsenceSchedule_Id");
            dataTable.Columns.Add("StartDate");
            dataTable.Columns.Add("EndDate");
            dataTable.Columns.Add("Absence_Id");
            dataTable.Columns.Add("SubstituteId");
            dataTable.Columns.Add("AbsenceStatus_Id");
            dataTable.Columns.Add("CreatedDate");
            if (StartDate.Date == EndDate.Date)
            {
                StartDate = StartDate.Add(startTime.TimeOfDay);
                EndDate = EndDate.Add(endTime.TimeOfDay);
                dataTable.Rows.Add(1, StartDate, EndDate, AbsenceId, DBNull.Value, 1, DateTime.Now);
            }
            else
            {
                var Days = (EndDate - StartDate).TotalDays;
                try
                {
                    DateTime startDateForStoringInDataTable;
                    DateTime endDateForStoringInDataTable;
                    for (int i = 1; i <= Days + 1; i++)
                    {
                        // For multi Day e.g if start time starts from today and ends on next day
                        if (endTime.TimeOfDay <= startTime.TimeOfDay)
                        {
                            //For First Day
                            if (i == 1)
                            {
                                startDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(startTime.TimeOfDay);
                                endDateForStoringInDataTable = StartDate.AddDays(i - 1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                                dataTable.Rows.Add(i, startDateForStoringInDataTable, endDateForStoringInDataTable, AbsenceId, DBNull.Value, 1, DateTime.Now);
                            }
                            //For Last Day
                            else if (i == Days + 1)
                            {
                                startDateForStoringInDataTable = StartDate.AddDays(i - 1).Date;
                                endDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(endTime.TimeOfDay);
                                dataTable.Rows.Add(i, startDateForStoringInDataTable, endDateForStoringInDataTable, AbsenceId, DBNull.Value, 1, DateTime.Now);
                            }
                            // Between First and Last Date
                            else
                            {
                                startDateForStoringInDataTable = StartDate.AddDays(i - 1).Date;
                                endDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(endTime.TimeOfDay);
                                dataTable.Rows.Add(i, startDateForStoringInDataTable, endDateForStoringInDataTable, AbsenceId, DBNull.Value, 1, DateTime.Now);
                                startDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(startTime.TimeOfDay);
                                endDateForStoringInDataTable = StartDate.AddDays(i - 1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                                dataTable.Rows.Add(i, startDateForStoringInDataTable, endDateForStoringInDataTable, AbsenceId, DBNull.Value, 1, DateTime.Now);
                            }
                            //For Multi day e.g if start time and end time is on same date
                        }
                        else
                        {
                            startDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(startTime.TimeOfDay);
                            endDateForStoringInDataTable = StartDate.AddDays(i - 1).Add(endTime.TimeOfDay);
                            dataTable.Rows.Add(i, startDateForStoringInDataTable, endDateForStoringInDataTable, AbsenceId, DBNull.Value, 1, DateTime.Now);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return dataTable;
        }

        #endregion
    }
}
