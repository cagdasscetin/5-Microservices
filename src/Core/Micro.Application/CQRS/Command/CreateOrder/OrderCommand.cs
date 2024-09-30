using MediatR;

namespace Micro.Application.CQRS.Command.CreateOrder;

public class OrderCommand : IRequest<OrderResponse>
{
    public int BuyerId { get; set; }
    public List<OrderItemCommand> OrderItems { get; set; }
}

public class OrderItemCommand
{
    public int Count { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
}