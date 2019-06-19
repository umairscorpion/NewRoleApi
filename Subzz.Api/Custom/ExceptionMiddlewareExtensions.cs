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

namespace Subzz.Api.Custom
{
    public static class ExceptionMiddlewareExtensions
    {
        
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IErrorLogService service, IUserService userService)
        {
            
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                        
                        
                    }
                });
            });
        }
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
