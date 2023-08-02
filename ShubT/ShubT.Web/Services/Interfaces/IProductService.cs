using ShubT.Web.Models;
using ShubT.Web.Models.Product;

namespace ShubT.Web.Services.Interfaces
{
    public interface IProductService
    {

        Task<ResponseDTO> GetAllProductsAsync();
        Task<ResponseDTO> GetProductByIdAsync(int id);
        Task<ResponseDTO> CreateProductsAsync(ProductDTO productDto);
        Task<ResponseDTO> UpdateProductsAsync(ProductDTO productDto);
        Task<ResponseDTO> DeleteProductsAsync(int id);
    }
}
