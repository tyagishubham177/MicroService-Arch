using Microsoft.EntityFrameworkCore;
using ShubT.Services.EmailAPI.Data;
using ShubT.Services.EmailAPI.Extensions;
using ShubT.Services.EmailAPI.Messaging;
using ShubT.Services.EmailAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionsBuilder.Options));

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
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

app.UseAzureServiceBusConsumer();

app.Run();

void ApplyMigrations()
{
    using var serviceScope = app.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}