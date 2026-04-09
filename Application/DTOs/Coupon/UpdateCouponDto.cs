namespace Application.DTOs.Coupon;

public class UpdateCouponDto
{
    public bool IsActive { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int UsageLimit { get; set; }
}
