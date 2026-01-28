using FluentValidation;
using OrderService.WebApi.DTO.Requests;

namespace OrderService.WebApi.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(1000);
        RuleFor(x => x.EmailClient).NotEmpty().EmailAddress();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?[1-9]\d{1,14}$");
    }
}