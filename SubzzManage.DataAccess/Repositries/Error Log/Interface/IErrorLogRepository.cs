﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzManage.DataAccess.Repositries.Error_Log.Interface
{
    public interface IErrorLogRepository
    {
        int InsertError(string userId, DateTime creationDate, int statusCode, string message, string messageDetail);
        int InsertEmailLog(string emailTo, string message, string subject, string exception, DateTime updatedOn, string absenceId, string statusCode);
    }
}
