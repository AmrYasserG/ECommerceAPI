using ECommerceAPI.BLL.DTOs;
using FluentValidation;

namespace ECommerceAPI.BLL.Validators
{
    public class UpdateCartDtoValidator : AbstractValidator<UpdateCartDto>
    {
        public UpdateCartDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("A valid ProductId is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
