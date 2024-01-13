using Tiba.OME.Domain.Core;

namespace Tiba.OME.Domain.OrderBookAgg.Events;

public class OrderPlacedDomainEvent: DomainEvent 
{
    public Guid OrderId { get; set; } 
    public  OrderSide OrderSide { get;  set; } 
    public  int Quantity { get;  set; } 
    public decimal Price { get;  set; } 
    public string CustomerCode { get;  set; } 
    public string InstrumentCode { get;  set; } 
    public new Guid EventId { get; }

    public OrderPlacedDomainEvent(IOrder order)
    {
        this.OrderId = order.Id;
        this.Quantity = order.Quantity;
        this.Price = order.Price;
        this.CustomerCode = order.CustomerCode;
        this.InstrumentCode = order.InstrumentCode;
        this.OrderSide = order.OrderSide;
    }
}