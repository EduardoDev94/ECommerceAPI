namespace Application.DTOs.Order;

/// <summary>
/// Valores válidos: Pending, Paid, Processing, Shipped, Delivered, Cancelled
/// </summary>
public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
