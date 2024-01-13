using Tiba.OME.Domain.Core;

namespace Tiba.OME.Domain.OrderBookAgg.Events;

public class OrderCancelledDomainEvent(IOrder order)  : DomainEvent
{
    public Guid OrderId { get; set; } = order.Id;
    public string InstrumentCode { get;  set; } = order.InstrumentCode;
    public new Guid EventId { get; }
}