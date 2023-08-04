using Microsoft.EntityFrameworkCore;
using ShubT.Services.EmailAPI.Data;
using ShubT.Services.EmailAPI.DTOs;
using ShubT.Services.EmailAPI.Message;
using ShubT.Services.EmailAPI.Models;
using System.Text;

namespace ShubT.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            //TODO : Make this fancy HTML
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDTO.CartHeaderDTO.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDTO.CartDetailsDTO)
            {
                message.Append("<li>");
                message.Append(item.ProductDTO.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDTO.CartHeaderDTO.Email);
        }

        public async Task LogOrderPlaced(RewardsMessage rewardsDto)
        {
            string message = "New Order Placed. <br/> Order ID : " + rewardsDto.OrderId;
            await LogAndEmail(message, rewardsDto.UserId);
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registeration Successful. <br/> Email : " + email;
            await LogAndEmail(message, email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}