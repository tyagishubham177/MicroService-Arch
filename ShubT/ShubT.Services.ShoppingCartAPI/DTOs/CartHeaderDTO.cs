namespace ShubT.Services.ShoppingCartAPI.DTOs
{
    public class CartHeaderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountTotal { get; set; }
        public double CartTotal { get; set; }

        //[Required]
        public string? Name { get; set; }
        //[Required]
        public string? Phone { get; set; }
        //[Required]
        public string? Email { get; set; }
    }
}
