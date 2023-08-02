using ShubT.Web.Models;

namespace ShubT.Web.Services.Interfaces
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
    }
}
