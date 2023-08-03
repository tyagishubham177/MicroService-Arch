using ShubT.Services.ShoppingCartAPI.DTOs;

namespace ShubT.Services.ShoppingCartAPI.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
}
