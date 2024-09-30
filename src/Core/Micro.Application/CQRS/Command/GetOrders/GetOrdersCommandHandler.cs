using MediatR;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Enums;

namespace Micro.Application.CQRS.Command.GetOrders;

public class GetOrdersCommandHandler : IRequestHandler<GetOrdersCommand, List<GetOrdersResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public GetOrdersCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<List<GetOrdersResponse>> Handle(GetOrdersCommand request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync();

        var orderItems = await _orderItemRepository.GetAllAsync();

        var orderResponseList = new List<GetOrdersResponse>();

        foreach (var order in orders)
        {
            var orderResponse = new GetOrdersResponse()
            {
                BuyerId = order.BuyerId,
                OrderItems = orderItems.Where(x => x.OrderId == order.Id).Select(oi => new OrderItemResponse()
                {
                    Count = oi.Count,
                    Price = oi.Price,
                    ProductId = oi.ProductId
                }).ToList(),
                OrderStatus = OrderStatusEnum.Suspend,
                TotalPrice = orderItems.Where(x => x.OrderId == order.Id).Sum(oi => oi.Count * oi.Price),
                CreatedDate = DateTime.UtcNow
            };
            orderResponseList.Add(orderResponse);
        }

        return orderResponseList;
    }
}