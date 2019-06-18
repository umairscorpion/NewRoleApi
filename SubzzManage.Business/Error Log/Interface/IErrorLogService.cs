using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzManage.Business.Error_Log.Interface
{
    public interface IErrorLogService
    {
        int InsertError(string userId, DateTime creationDate, int statusCode, string message, string messageDetail);
    }
}
