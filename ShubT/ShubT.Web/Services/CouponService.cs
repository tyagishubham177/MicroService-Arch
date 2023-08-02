using ShubT.Web.Models;
using ShubT.Web.Models.Coupons;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;

namespace ShubT.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> CreateCouponAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = couponDTO,
                Url = MiscUtils.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = MiscUtils.CouponAPIBase + "/api/coupon/" + couponId
            });
        }

        public async Task<ResponseDTO> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                Url = MiscUtils.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO> GetCouponAsync(string code)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                Url = MiscUtils.CouponAPIBase + "/api/coupon/GetCouponByCode/" + code
            });
        }

        public async Task<ResponseDTO> GetCouponByIdAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                Url = MiscUtils.CouponAPIBase + "/api/coupon/" + couponId
            });
        }

        public async Task<ResponseDTO> UpdateCouponsAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = couponDTO,
                Url = MiscUtils.CouponAPIBase + "/api/coupon"
            });
        }
    }
}
