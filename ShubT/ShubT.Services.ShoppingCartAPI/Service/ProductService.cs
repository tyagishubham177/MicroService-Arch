using Newtonsoft.Json;
using ShubT.Services.ShoppingCartAPI.DTOs;
using ShubT.Services.ShoppingCartAPI.Service.Interfaces;

namespace ShubT.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var client = _clientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (apiResponse.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(apiResponse.Result.ToString());
                return result;
            }
            else
            {
                return new List<ProductDTO>();
            }
        }
    }
}
