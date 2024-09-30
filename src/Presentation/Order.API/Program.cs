using MassTransit;
using Micro.Application.Extensions;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;
using Micro.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) =>
    {
        var host = builder.Configuration.GetSection("RabbitMQ:Host").Value;
        _configure.Host(host, "/", h =>
        {
            h.Username(builder.Configuration.GetSection("RabbitMQ:Username").Value);
            h.Password(builder.Configuration.GetSection("RabbitMQ:Password").Value);
        });
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
