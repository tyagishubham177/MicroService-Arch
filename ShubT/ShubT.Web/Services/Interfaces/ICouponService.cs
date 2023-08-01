using ShubT.Web.Models;

namespace ShubT.Web.Services.Interfaces
{
    public interface ICouponService
    {
        Task<ResponseDTO> GetCouponAsync(string code);
        Task<ResponseDTO> GetAllCouponsAsync();
        Task<ResponseDTO> GetCouponByIdAsync(int couponId);
        Task<ResponseDTO> CreateCouponAsync(CouponDTO couponDTO);
        Task<ResponseDTO> UpdateCouponsAsync(CouponDTO couponDTO);
        Task<ResponseDTO> DeleteCouponAsync(int couponId);
    }
}
