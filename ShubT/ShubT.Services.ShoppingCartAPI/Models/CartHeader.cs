using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShubT.Services.ShoppingCartAPI.Models
{
    public class CartHeader
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        [NotMapped]
        public double DiscountTotal { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
