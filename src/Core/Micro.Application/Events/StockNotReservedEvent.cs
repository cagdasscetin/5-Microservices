using Micro.Application.Events.Base;

namespace Micro.Application.Events;

public class StockNotReservedEvent : IEvent
{
    public int OrderId { get; set; }
    public int BuyerId { get; set; }
    public string Message { get; set; }
}