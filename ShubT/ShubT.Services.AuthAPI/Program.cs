using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShubT.Services.AuthAPI.Data;
using ShubT.Services.AuthAPI.Models;
using ShubT.Services.AuthAPI.Services;
using ShubT.Services.AuthAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("APISettings:JWTOptions"));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigrations();

app.Run();

void ApplyMigrations()
{
    using var serviceScope = app.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}

