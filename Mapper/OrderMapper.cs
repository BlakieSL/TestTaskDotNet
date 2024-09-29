using TestTask.DTO;
using TestTask.Helper;
using TestTask.Model;

namespace TestTask.Mapper;

public class OrderMapper : IOrderMapper
{
    public OrderResponseDto ToOrderResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            Status = order.Status.ToString(),
            Prioriry = order.Priority,
            TotalAmountInBaseCurrency = order.TotalAmountInBaseCurrency ?? 0
        };
    }

    public Order ToOrderEntity(OrderCreateDto dto)
    {
        return new Order
        {
            CustomerName = dto.CustomerName,
            OrderDate = DateTime.Now,
            TotalAmount = dto.TotalAmount,
            Currency = dto.Currency,
            Status = Status.Pending,
            Priority = CalculationHelper.CalculateOrderPriority(dto.TotalAmount, DateTime.Now),
        };
    }

    public void UpdateOrder(Order order, OrderUpdateDto dto)
    {
        if (dto.CustomerName != null)
        {
            order.CustomerName = dto.CustomerName;
        }
        
        if (dto.TotalAmount.HasValue)
        {
            order.TotalAmount = dto.TotalAmount.Value;
        }
        
        if (dto.Currency != null)
        {
            order.Currency = dto.Currency;
        }
        
        if (dto.Status.HasValue)
        {
            order.Status = dto.Status.Value;
        }
    }
}