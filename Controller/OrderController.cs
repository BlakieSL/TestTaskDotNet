using Microsoft.AspNetCore.Mvc;
using TestTask.DTO;
using TestTask.Service;

namespace TestTask.Controller;

[ApiController]
[Route("api/orders/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderById(id);
        return Ok(order);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
    {
        var createdOrder = await _orderService.CreateOrder(dto);
        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOrderById(int id, [FromBody] OrderUpdateDto dto)
    {
        var updatedOrder = await _orderService.UpdateOrderById(id, dto);
        return Ok(updatedOrder);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderById(int id)
    {
        await _orderService.DeleteOrderById(id);
        return NoContent();
    }
}