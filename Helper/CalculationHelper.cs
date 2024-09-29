namespace TestTask.Helper;

public class CalculationHelper
{
    public static int CalculateOrderPriority(decimal totalAmount, DateTime orderDate)
    {
        var minutesPassed = (DateTime.Now - orderDate).TotalMinutes;
        
        var bonusPoints = (int)(minutesPassed / 10);
        
        return (int)(totalAmount + bonusPoints);
    }
}