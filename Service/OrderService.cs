using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TestTask.Context;
using TestTask.DTO;
using TestTask.Mapper;
using TestTask.Model;

namespace TestTask.Service;

public class OrderService : IOrderService
{
    private readonly MyDbContext _context;
    private readonly IOrderMapper _orderMapper;
    private readonly HttpClient _httpClient;
    
    public OrderService(MyDbContext context, IOrderMapper orderMapper, HttpClient httpClient)
    {
        _context = context;
        _orderMapper = orderMapper;
        _httpClient = httpClient;
    }
    
    public async Task<OrderResponseDto> GetOrderById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with Id {id} not found");
        }

        return _orderMapper.ToOrderResponseDto(order);
    }

    public async Task<OrderResponseDto> CreateOrder(OrderCreateDto dto)
    {
        var order = _orderMapper.ToOrderEntity(dto);
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return _orderMapper.ToOrderResponseDto(order);
    }

    public async Task<OrderResponseDto> UpdateOrderById(int id, OrderUpdateDto dto)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with Id {id} not found");
        }

        _orderMapper.UpdateOrder(order, dto);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        var updatedOrder = await _context.Orders.FindAsync(id);
        
        if (updatedOrder == null)
        {
            throw new KeyNotFoundException($"Order with Id {id} not found");
        }
        
        return _orderMapper.ToOrderResponseDto(updatedOrder);
    }

    public async Task DeleteOrderById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with Id {id} not found");
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    public async Task ProcessPending()
    {
        var pendingOrders = await _context.Orders
            .Where(order => order.Status == Status.Pending)
            .OrderByDescending(order => order.Priority)
            .ToListAsync();

        foreach (var order in pendingOrders)
        {
            var conversionRate = await ConvertToUsdAsync(order.Currency);
            order.TotalAmountInBaseCurrency = order.TotalAmount / conversionRate;
            

            order.Status = Status.Processing;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task ProcessProcessingOrders()
    {
        var processingOrders = await _context.Orders
            .Where(order => order.Status == Status.Processing)
            .ToListAsync();

        foreach (var order in processingOrders)
        {
            order.Status = Status.Completed;
            await _context.SaveChangesAsync();

            LogOrder(order);
        }
    }
    
    public async Task RecalculatePriorities()
    {
        var pendingOrders = await _context.Orders
            .Where(order => order.Status == Status.Pending)
            .ToListAsync();

        foreach (var order in pendingOrders)
        {
            order.Priority += (int)(DateTime.Now - order.OrderDate).TotalMinutes / 60;
        }

        await _context.SaveChangesAsync();
    }
    
    private async Task<decimal> ConvertToUsdAsync(string currency)
    {
        var data = JObject
            .Parse(await _httpClient
                .GetStringAsync("https://v6.exchangerate-api.com/v6/d32e8c2b23a1474352887ed6/latest/USD"));

        if (data["conversion_rates"]?[currency] == null)
        {
            throw new Exception("Invalid currency or failed to fetch exchange rate.");
        }

        return data["conversion_rates"]![currency].Value<decimal>();
    }
    
    private void LogOrder(Order order)
    {
        var logMessage = $"{DateTime.Now}; {order.Id}; {order.CustomerName}; {order.TotalAmount}; {order.Currency}; {order.TotalAmountInBaseCurrency};" + Environment.NewLine;

        File.AppendAllText("Logs.txt", logMessage);
    }
}