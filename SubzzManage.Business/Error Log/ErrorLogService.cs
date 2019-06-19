using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SubzzManage.Business.Error_Log.Interface;
using SubzzManage.DataAccess.Repositries.Error_Log.Interface;
using SubzzManage.DataAccess.Repositries.Error_Log;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SubzzManage.Business.Error_Log
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IErrorLogRepository _repo;
        public ErrorLogService()
        {
            _repo = new ErrorLogRepository();
        }
        int IErrorLogService.InsertError(string userId, DateTime creationDate, int statusCode, string message, string messageDetail)
        {
            var error = _repo.InsertError(userId, creationDate, statusCode, message, messageDetail);
            return error;
        }
    }
}
