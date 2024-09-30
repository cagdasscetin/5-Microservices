using Micro.Application.Events.Base;

namespace Micro.Application.Events;

public class PaymentCompletedEvent : IEvent
{
    public int OrderId { get; set; }
}