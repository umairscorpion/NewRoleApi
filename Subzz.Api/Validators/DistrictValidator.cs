using FluentValidation;
using SubzzV2.Core.Models;

namespace Subzz.Api.Validators
{
    public class DistrictValidator : AbstractValidator<DistrictModel>
    {
        public DistrictValidator()
        {
            RuleFor(m => m.DistrictName).NotNull().NotEmpty();
            RuleFor(m => m.DistrictStateId).NotNull().NotEmpty();
            RuleFor(m => m.City).NotNull().NotEmpty();
            RuleFor(m => m.DistrictAddress).NotNull().NotEmpty();
            RuleFor(m => m.DistrictPhone).NotNull().NotEmpty();
            RuleFor(m => m.DistrictStartTime).NotNull().NotEmpty();
            RuleFor(m => m.DistrictEndTime).NotNull().NotEmpty();
            RuleFor(m => m.District1stHalfEnd).NotNull().NotEmpty();
            RuleFor(m => m.District2ndHalfStart).NotNull().NotEmpty();
            RuleFor(m => m.DistrictZipCode).NotNull().NotEmpty();
        }
    }
}
