using Micro.Application.Events.Base;
using Micro.Domain.Messages;

namespace Micro.Application.Events;

public class StockReservedEvent : IEvent
{
    public int OrderId { get; set; }
    public int BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}