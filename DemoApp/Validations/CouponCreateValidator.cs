using DemoApp.Models.DTO;
using FluentValidation;

namespace DemoApp.Validations
{
    public class CouponCreateValidator: AbstractValidator<CreateCouponDTO>
    {
        public CouponCreateValidator() 
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.percent).InclusiveBetween(1, 100);
        }
    }
}
