namespace TestTask.DTO;

public class OrderCreateDto
{
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
}