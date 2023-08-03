using Microsoft.EntityFrameworkCore;
using ShubT.Services.EmailAPI.Models;

namespace ShubT.Services.EmailAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}