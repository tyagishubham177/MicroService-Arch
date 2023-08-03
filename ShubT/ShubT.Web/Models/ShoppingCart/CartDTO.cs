namespace ShubT.Web.Models.ShoppingCart
{
    public class CartDTO
    {
        public CartHeaderDTO CartHeaderDTO { get; set; }
        public IEnumerable<CartDetailsDTO> CartDetailsDTO { get; set; }
    }
}
