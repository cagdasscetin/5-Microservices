using MassTransit;
using MediatR;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Domain.Enums;
using Micro.Domain.Messages;
using Newtonsoft.Json;

namespace Micro.Application.CQRS.Command.CreateOrder;

public class OrderCommandHandler : IRequestHandler<OrderCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOutboxRepository _outboxRepository;

    public OrderCommandHandler(IOrderRepository orderRepository, IOutboxRepository outboxRepository)
    {
        _orderRepository = orderRepository;
        _outboxRepository = outboxRepository;
    }

    public async Task<OrderResponse> Handle(OrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order()
        {
            BuyerId = request.BuyerId,
            OrderItems = request.OrderItems.Select(oi => new OrderItem()
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId
            }).ToList(),
            OrderStatus = OrderStatusEnum.Suspend,
            TotalPrice = request.OrderItems.Sum(oi => oi.Count * oi.Price),
            CreatedDate = DateTime.UtcNow
        };

        order = await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        OrderCreatedEvent orderCreatedEvent = new()
        {
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            TotalPrice = order.TotalPrice,
            OrderItems = order.OrderItems.Select(oi => new OrderItemMessage()
            {
                Price = oi.Price,
                Count = oi.Count,
                ProductId = oi.ProductId
            }).ToList()
        };

        var newMessage = new OutboxMessage()
        {
            
            EventType = orderCreatedEvent.GetType().ToString(),
            EventPayload = JsonConvert.SerializeObject(orderCreatedEvent)
        };
        
        var outMessage = await _outboxRepository.AddAsync(newMessage);
        await _outboxRepository.SaveChangesAsync();

        return new OrderResponse();
    }
}