using TestTask.Model;

namespace TestTask.DTO;

public class OrderUpdateDto
{
    public string? CustomerName { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public Status? Status { get; set; }
}