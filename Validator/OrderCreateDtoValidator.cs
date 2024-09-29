using FluentValidation;
using TestTask.DTO;

namespace TestTask.Validator;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(order => order.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.");

        RuleFor(order => order.TotalAmount)
            .GreaterThan(0).WithMessage("Should be greater than zero.");
/*
        RuleFor(order => order.OrderDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Date can not be in future.");
*/
        RuleFor(order => order.Currency)
            .Must(IsValidCurrency).WithMessage("Unsupported currency.");
    }

    private bool IsValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR" };
        return validCurrencies.Contains(currency);
    }
}