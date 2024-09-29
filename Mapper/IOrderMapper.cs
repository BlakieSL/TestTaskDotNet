using TestTask.DTO;
using TestTask.Model;

namespace TestTask.Mapper;

public interface IOrderMapper
{
    OrderResponseDto ToOrderResponseDto(Order order);
    Order ToOrderEntity(OrderCreateDto dto);
    void UpdateOrder(Order order, OrderUpdateDto dto);
}