using MassTransit;
using Micro.Application.Events;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Messages;

namespace Stock.Consumer.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IStockRepository _stockRepository;

    public PaymentFailedEventConsumer(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var list = await _stockRepository.GetAllAsync();
        foreach (OrderItemMessage orderItem in context.Message.OrderItems)
        {
            var stock = list.FirstOrDefault(s => s.ProductId == orderItem.ProductId);
            if (stock != null)
            {
                stock.Count += orderItem.Count;
                var temp = list.FirstOrDefault(s => s.ProductId == orderItem.ProductId);

                var updateStock = await _stockRepository.GetAsync(x => x.ProductId == orderItem.ProductId);
                updateStock.Count = stock.Count;
                await _stockRepository.UpdateAsync(updateStock);
            }
        }
    }
}