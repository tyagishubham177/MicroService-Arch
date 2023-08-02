using System.ComponentModel.DataAnnotations;

namespace ShubT.Web.Models.Coupons
{
    public class CouponDTO
    {
        public int Id { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
