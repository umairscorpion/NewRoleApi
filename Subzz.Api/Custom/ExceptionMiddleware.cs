using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SubzzV2.Core.Models;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Subzz.Api.Controllers.Base;
using SubzzManage.Business.Error_Log.Interface;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Subzz.Api.Custom;
using System.Data;
using Subzz.Integration.Core.Container;
using SubzzV2.Core;
using SubzzV2.Core.Enum;
using Subzz.Business.Services.Users.Interface;
using SubzzManage.Business.Error_Log;
using Subzz.Integration.Core.Helper;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;

namespace Subzz.Api.Custom
{
    public class ExceptionMiddleware   
    {
        private readonly RequestDelegate _next;
        private readonly IErrorLogService _service;
        private readonly IUserService _userService;
        private IHostingEnvironment _hostingEnvironment;
        public ExceptionMiddleware(RequestDelegate next, IErrorLogService service, IHostingEnvironment hostingEnvironment, IUserService userService)
        {
            _next = next;
            _service = service;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
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
            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error."

            }.ToString());
           

        }
    }
}
