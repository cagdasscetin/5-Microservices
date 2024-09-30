using MassTransit;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain;
using Micro.Domain.Entities;
using Micro.Domain.Messages;
using Micro.Domain.Settings;
using Newtonsoft.Json;

namespace Stock.Consumer.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IStockRepository _stockRepository;
    private readonly IOutboxRepository _outboxRepository;

    public OrderCreatedEventConsumer(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, IStockRepository stockRepository, IOutboxRepository outboxRepository)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
        _stockRepository = stockRepository;
        _outboxRepository = outboxRepository;
    }
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResult = new();
        var list = await _stockRepository.GetAllAsync();

        foreach (OrderItemMessage orderItem in context.Message.OrderItems)
        {
            stockResult.Add(list.Any(s => s.ProductId == orderItem.ProductId && s.Count > orderItem.Count));

            var s = list.Where(s => s.ProductId == orderItem.ProductId);
            var c = list.FirstOrDefault();
        }

        if (stockResult.TrueForAll(sr => sr.Equals(true)))
        {
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                var stock = list.FirstOrDefault(s => s.ProductId == orderItem.ProductId);
                stock.Count -= orderItem.Count;
                
                var updateStock = await _stockRepository.GetAsync(x => x.ProductId == orderItem.ProductId);
                updateStock.Count = stock.Count;
                await _stockRepository.UpdateAsync(updateStock);
            }

            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
            StockReservedEvent stockReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                OrderItems = context.Message.OrderItems,
                TotalPrice = context.Message.TotalPrice,
            };
            
            var newMessage = new OutboxMessage()
            {
                EventType = stockReservedEvent.GetType().ToString(),
                EventPayload = JsonConvert.SerializeObject(stockReservedEvent)
            };
        
            var outMessage = await _outboxRepository.AddAsync(newMessage);
            await _outboxRepository.SaveChangesAsync();
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                Message = "Yetersiz stok"
            };
            
            var newMessage = new OutboxMessage()
            {
                EventType = stockNotReservedEvent.GetType().ToString(),
                EventPayload = JsonConvert.SerializeObject(stockNotReservedEvent)
            };
        
            var outMessage = await _outboxRepository.AddAsync(newMessage);
            await _outboxRepository.SaveChangesAsync();
        }
    }
}