using ShubT.Web.Models;
using ShubT.Web.Models.ShoppingCart;
using ShubT.Web.Models.Stripe;

namespace ShubT.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDTO> CreateOrder(CartDTO cartDto);
        Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO> GetAllOrder(string? userId);
        Task<ResponseDTO> GetOrder(int orderId);
        Task<ResponseDTO> UpdateOrderStatus(int orderId, string newStatus);
        Task<ResponseDTO> ValidateStripeSession(int orderHeaderId);
    }
}
