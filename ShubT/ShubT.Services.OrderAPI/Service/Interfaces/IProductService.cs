using ShubT.Services.OrderAPI.DTOs;

namespace ShubT.Services.OrderAPI.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
}
