using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Subzz.Api.Validators;
using Subzz.Business.Services.Users;
using Subzz.Business.Services.Users.Interface;
using Subzz.DataAccess.Repositories.Users;
using Subzz.DataAccess.Repositories.Users.Interface;
using SubzzAbsence.Business.Absence;
using SubzzAbsence.Business.Absence.Interface;
using SubzzAbsence.Business.Leaves;
using SubzzAbsence.Business.Leaves.Interface;
using SubzzAbsence.DataAccess.Repositories.Absence;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Base;
using SubzzAbsence.DataAccess.Repositories.Leaves;
using SubzzAbsence.DataAccess.Repositories.Leaves.Interface;
using SubzzLookup.Business.Lookups;
using SubzzLookup.Business.Lookups.Interface;
using SubzzLookup.DataAccess.Repositories;
using SubzzLookup.DataAccess.Repositories.Interface;
using SubzzLookup.DataAccess.Repositories.Lookups;
using SubzzLookup.DataAccess.Repositories.Lookups.Interface;
using SubzzManage.Business.District;
using SubzzManage.Business.District.Interface;
using SubzzManage.Business.Manage;
using SubzzManage.Business.Manage.Interface;
using SubzzManage.DataAccess.Repositries.Manage;
using SubzzManage.DataAccess.Repositries.Manage.Interface;

namespace Subzz.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidatorActionFilter));
            }).AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build();
                });
            });
            services.AddTransient<IRepository, EntityRepository>();
            services.AddTransient<IUserAuthenticationService, UserAuthenticationService>();
            services.AddTransient<IUserRepository, UserRepository>(); 
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<ILeaveService, LeaveService>();
            services.AddTransient<ILeaveRepository, LeaveRepository>();

            services.AddTransient<IAbsenceRepository, AbsenceRepository>();
            services.AddTransient<IAbsenceService, AbsenceService>();

            services.AddTransient<IDistrictLookupRepository, DistrictLookupRepository>();
            services.AddTransient<IDistrictLookupService, DistrictLookupService>();
            
            services.AddTransient<ILookupRepository, LookupRepository>();
            services.AddTransient<ILookupService, LookupService>();

            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IDistrictService, DistrictService>();

            services.AddTransient<ISchoolRepository, SchoolRepository>();
            services.AddTransient<ISchoolService, SchoolService>();

            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IJobRepository, JobRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = "http://localhost:61137",
                   ValidAudience = "http://localhost:61137",
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("P@$sw0rd123Ki@Keysec"))
               };
           });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("EnableCORS");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
