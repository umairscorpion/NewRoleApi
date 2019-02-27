using FluentValidation;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Validators
{
    public class AbsenceValidator : AbstractValidator<AbsenceModel>
    {
        public AbsenceValidator()
        {
            RuleFor(m => m.DistrictId).NotEmpty();
        }
    }
}
