using ShubT.Web.Models;
using ShubT.Web.Models.ShoppingCart;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;
using static ShubT.Web.Models.ProjectEnums;

namespace ShubT.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = MiscUtils.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDTO> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = MiscUtils.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
            });
        }

        public async Task<ResponseDTO> GetCartByUserIdAsnyc(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = MiscUtils.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId
            });
        }


        public async Task<ResponseDTO> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDetailsId,
                Url = MiscUtils.ShoppingCartAPIBase + "/api/cart/RemoveCart"
            });
        }


        public async Task<ResponseDTO> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = MiscUtils.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
    }
}
