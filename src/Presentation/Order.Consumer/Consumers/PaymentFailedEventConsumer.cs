using MassTransit;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Enums;

namespace Order.Consumer.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IOrderRepository _orderRepository;

    public PaymentFailedEventConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var order = await _orderRepository.GetAsync(x => x.Id == context.Message.OrderId);
        if (order != null)
        {
            order.OrderStatus = OrderStatusEnum.Fail;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}