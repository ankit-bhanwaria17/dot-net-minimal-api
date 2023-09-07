using DemoApp.Models.DTO;
using FluentValidation;

namespace DemoApp.Validations
{
    public class UpdateCouponValidator:AbstractValidator<UpdateCouponDTO>
    {
        public UpdateCouponValidator()
        {
            RuleFor(model => model.Id).NotEqual(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.percent).InclusiveBetween(1, 100);
        }
    }
}
