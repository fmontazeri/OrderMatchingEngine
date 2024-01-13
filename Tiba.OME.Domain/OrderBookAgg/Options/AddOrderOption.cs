namespace Tiba.OME.Domain.OrderBookAgg.Options;

public class AddOrderOption(
    OrderSide orderSide,
    int quantity,
    decimal price,
    string customerCode,
    string instrumentCode)
    : IOrderOptions
{
    public OrderSide OrderSide { get; } = orderSide;
    public int Quantity { get; } = quantity;
    public decimal Price { get; } = price;
    public string CustomerCode { get; } = customerCode;
    public string InstrumentCode { get; } = instrumentCode;
}


