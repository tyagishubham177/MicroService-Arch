using ShubT.Web.Models;
using ShubT.Web.Models.Product;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;
using static ShubT.Web.Models.ProjectEnums;

namespace ShubT.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> CreateProductsAsync(ProductDTO productDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = productDto,
                Url = MiscUtils.ProductAPIBase + "/api/product",
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDTO> DeleteProductsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = MiscUtils.ProductAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = MiscUtils.ProductAPIBase + "/api/product"
            });
        }



        public async Task<ResponseDTO> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = MiscUtils.ProductAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO> UpdateProductsAsync(ProductDTO productDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = productDto,
                Url = MiscUtils.ProductAPIBase + "/api/product",
                ContentType = ContentType.MultipartFormData
            });
        }
    }
}