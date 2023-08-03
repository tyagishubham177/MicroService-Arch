namespace ShubT.Services.ShoppingCartAPI.DTOs
{
    public class CartDetailsDTO
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDTO? CartHeaderDTO { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? ProductDTO { get; set; }
        public int Count { get; set; }
    }
}
