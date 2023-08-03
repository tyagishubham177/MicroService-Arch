using Newtonsoft.Json;
using ShubT.Services.ShoppingCartAPI.DTOs;
using ShubT.Services.ShoppingCartAPI.Service.Interfaces;

namespace ShubT.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _clientFactory;

        public CouponService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<CouponDTO>> GetAllCouponsAsync()
        {
            var client = _clientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon");

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (apiResponse.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<IEnumerable<CouponDTO>>(apiResponse.Result.ToString());
                return result;
            }
            else
            {
                return new List<CouponDTO>();
            }
        }

        public async Task<CouponDTO> GetCouponsByCodeAsync(string couponCode)
        {
            var client = _clientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetCouponByCode/{couponCode}");

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (apiResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(apiResponse.Result.ToString()); ;
            }
            else
            {
                return new CouponDTO();
            }
        }
    }
}
