using FluentValidation;
using TestTask.DTO;

namespace TestTask.Validator;

public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateDtoValidator()
    {
        RuleFor(order => order.TotalAmount)
            .GreaterThan(0).When(order => order.TotalAmount.HasValue)
            .WithMessage("Should be Greater than zero when provided.");
      /*  
        RuleFor(order => order.OrderDate)
            .LessThanOrEqualTo(DateTime.Now).When(order => order.OrderDate.HasValue)
            .WithMessage("Date can not be in future when provided.");
      */
        
        RuleFor(order => order.Currency)
            .Must(IsValidCurrency).When(order => order.Currency != null)
            .WithMessage("Unsupported currency.");
        
        RuleFor(order => order.Status)
            .IsInEnum().When(order => order.Status.HasValue)
            .WithMessage("Selected status type doesn't exist.");
    }
    
    private bool IsValidCurrency(string? currency)
    {
        if (currency == null)
            return true;

        var validCurrencies = new[] { "USD", "EUR" };
        return validCurrencies.Contains(currency);
    }
}