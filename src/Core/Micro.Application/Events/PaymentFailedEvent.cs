using Micro.Application.Events.Base;
using Micro.Domain.Messages;

namespace Micro.Application.Events;

public class PaymentFailedEvent : IEvent
{
    public int OrderId { get; set; }
    public string Message { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}