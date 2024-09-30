using MassTransit;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Newtonsoft.Json;

namespace Payment.Consumer.Consumers;

public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOutboxRepository _outboxRepository;
    
    public StockReservedEventConsumer(IPublishEndpoint publishEndpoint, IOutboxRepository outboxRepository)
    {
        _publishEndpoint = publishEndpoint;
        _outboxRepository = outboxRepository;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        if (true)
        {
            PaymentCompletedEvent paymentCompletedEvent = new()
            {
                OrderId = context.Message.OrderId
            };
            
            var newMessage = new OutboxMessage()
            {
                EventType = paymentCompletedEvent.GetType().ToString(),
                EventPayload = JsonConvert.SerializeObject(paymentCompletedEvent)
            };
        
            var outMessage = await _outboxRepository.AddAsync(newMessage);
            await _outboxRepository.SaveChangesAsync();
            
            Console.WriteLine("Ödeme başarılı...");
        }
        else
        {
            PaymentFailedEvent paymentFailedEvent = new()
            {
                Message = "Bakiye yetersiz...",
                OrderId = context.Message.OrderId,
                OrderItems = context.Message.OrderItems
            };
            
            var newMessage = new OutboxMessage()
            {
                EventType = paymentFailedEvent.GetType().ToString(),
                EventPayload = JsonConvert.SerializeObject(paymentFailedEvent)
            };
        
            var outMessage = await _outboxRepository.AddAsync(newMessage);
            await _outboxRepository.SaveChangesAsync();
            
            Console.WriteLine("Ödeme başarısız...");
        }
    }
}