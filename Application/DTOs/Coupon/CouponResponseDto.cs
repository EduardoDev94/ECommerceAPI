namespace Application.DTOs.Coupon;

public class CouponResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public int UsageLimit { get; set; }
    public int TimesUsed { get; set; }
}
