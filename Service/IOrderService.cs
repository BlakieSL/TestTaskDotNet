using TestTask.DTO;

namespace TestTask.Service;

public interface IOrderService
{
    Task<OrderResponseDto> GetOrderById(int id);
    Task<OrderResponseDto> CreateOrder(OrderCreateDto dto);
    Task<OrderResponseDto> UpdateOrderById(int id, OrderUpdateDto dto);
    Task DeleteOrderById(int id);
    Task ProcessPending();
    Task ProcessProcessingOrders();
    Task RecalculatePriorities();
}