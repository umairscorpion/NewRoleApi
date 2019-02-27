using FluentValidation;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Validators
{
    public class OrganizationValidator : AbstractValidator<OrganizationModel>
    {
        public OrganizationValidator()
        {
            RuleFor(m => m.SchoolCity).NotNull().NotEmpty();
            RuleFor(m => m.SchoolDistrictId).NotNull().NotEmpty();
            RuleFor(m => m.SchoolAddress).NotNull().NotEmpty();
            RuleFor(m => m.SchoolPhone).NotNull().NotEmpty();
            RuleFor(m => m.SchoolTimeZone).NotNull().NotEmpty();
            RuleFor(m => m.SchoolStartTime).NotNull().NotEmpty();
            RuleFor(m => m.School1stHalfEnd).NotNull().NotEmpty();
            RuleFor(m => m.School2ndHalfStart).NotNull().NotEmpty();
            RuleFor(m => m.SchoolEndTime).NotNull().NotEmpty();
            RuleFor(m => m.SchoolZipCode).NotNull().NotEmpty();
        }
    }
}
