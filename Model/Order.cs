namespace TestTask.Model;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public Status Status { get; set; }
    public int Priority { get; set; }
    public decimal? TotalAmountInBaseCurrency { get; set; }
}