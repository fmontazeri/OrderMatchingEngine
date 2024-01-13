namespace Tiba.OME.Domain.OrderBookAgg;

public interface IOrderOptions
{
    public OrderSide OrderSide { get; }
    public int Quantity { get; }
    public decimal Price { get; }
    public string CustomerCode { get; }
    public string InstrumentCode { get; }

}