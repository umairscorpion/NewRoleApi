using Microsoft.AspNetCore.Http;
using Subzz.Business.Services.Users.Interface;
using SubzzManage.Business.Error_Log.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Utilities.ErrorHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IErrorLogService _service;
        private readonly IUserService _userService;
        public ExceptionMiddleware(RequestDelegate next, IErrorLogService service, IUserService userService)
        {
            _next = next;
            _service = service;
            _userService = userService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var currentDate = DateTime.Now;
                var userId = httpContext.User.FindFirst("UserId")?.Value;
                _service.InsertError(userId, currentDate, httpContext.Response.StatusCode, ex.Message, ex.StackTrace);

            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error."

            }.ToString());


        }
    }
}
