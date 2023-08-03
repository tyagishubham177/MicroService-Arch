using ShubT.Services.EmailAPI.DTOs;
using ShubT.Services.EmailAPI.Message;

namespace ShubT.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
