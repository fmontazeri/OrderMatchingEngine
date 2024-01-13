using Tiba.OME.Domain.Core;

namespace Tiba.OME.Domain.OrderBookAgg.Events;

public class OrderMatchedDomainEvent(int matchedQuantity , IOrder incomingOrder , IOrder queuedOrder) : DomainEvent
{
    public string InstrumentCode { get; set; } = incomingOrder.InstrumentCode;
    public int Quantity { get; set; } = matchedQuantity;
    public decimal Price { get; set; } = queuedOrder.Price;
    public Guid BuyOrderId { get; set; } = incomingOrder.OrderSide == OrderSide.Buy? incomingOrder.Id : queuedOrder.Id ;
    public string Buyer { get; set; } =  incomingOrder.OrderSide == OrderSide.Buy? incomingOrder.CustomerCode : queuedOrder.CustomerCode ;
    public Guid SellOrderId { get; set; } = incomingOrder.OrderSide == OrderSide.Sell? incomingOrder.Id : queuedOrder.Id ;
    public string Seller { get; set; } = incomingOrder.OrderSide == OrderSide.Sell? incomingOrder.CustomerCode : queuedOrder.CustomerCode ;
    public Guid EventId { get; }
  
}