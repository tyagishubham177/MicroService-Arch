namespace ShubT.Services.OrderAPI.DTOs
{
    public class CartDTO
    {
        public CartHeaderDTO CartHeaderDTO { get; set; }
        public IEnumerable<CartDetailsDTO> CartDetailsDTO { get; set; }
    }
}
