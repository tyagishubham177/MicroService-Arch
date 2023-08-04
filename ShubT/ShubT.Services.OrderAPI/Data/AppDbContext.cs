﻿using Microsoft.EntityFrameworkCore;
using ShubT.Services.OrderAPI.DTOs;
using ShubT.Services.OrderAPI.Models;

namespace ShubT.Services.OrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}