using ShubT.Services.ShoppingCartAPI.DTOs;

namespace ShubT.Services.ShoppingCartAPI.Service.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDTO>> GetAllCouponsAsync();
        Task<CouponDTO> GetCouponsByCodeAsync(string couponCode);
    }
}
