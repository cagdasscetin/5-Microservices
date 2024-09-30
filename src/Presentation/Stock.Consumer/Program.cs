using MassTransit;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain;
using Micro.Domain.Settings;
using Micro.Persistence.Context;
using Micro.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Stock.Consumer.Consumers;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        var host = builder.Configuration.GetSection("RabbitMQ:Host").Value;
        _configure.Host(host, "/", h =>
        {
            h.Username(builder.Configuration.GetSection("RabbitMQ:Username").Value);
            h.Password(builder.Configuration.GetSection("RabbitMQ:Password").Value);
        });

        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using IServiceScope scope = app.Services.CreateScope();

app.Run();
