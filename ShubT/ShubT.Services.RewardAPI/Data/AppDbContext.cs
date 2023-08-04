using Microsoft.EntityFrameworkCore;
using ShubT.Services.RewardAPI.Models;

namespace ShubT.Services.RewardAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rewards> Rewards { get; set; }
    }
}