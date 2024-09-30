using MassTransit;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Enums;

namespace Order.Consumer.Consumers;

public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IOrderRepository _orderRepository;
    
    public PaymentCompletedEventConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order = await _orderRepository.GetAsync(x => x.Id == context.Message.OrderId);
        if (order != null)
        {
            order.OrderStatus = OrderStatusEnum.Completed;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}