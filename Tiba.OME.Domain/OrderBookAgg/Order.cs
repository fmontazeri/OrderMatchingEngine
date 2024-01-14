using Tiba.OME.Domain.Core;
using Tiba.OME.Domain.OrderBookAgg.Exceptions;

namespace Tiba.OME.Domain.OrderBookAgg;

public class Order : EntityBase<Guid>, IOrder
{
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public OrderSide OrderSide { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public string CustomerCode { get; private set; }
    public string InstrumentCode { get; private set; }
    public OrderState OrderState { get; private set; }

    private Order(string instrumentCode, OrderSide orderSide, int quantity, decimal price,
        string customerCode)
    {
        if (quantity <= 0)
            throw new QuantityIsNotValidException();
        if (price <= 0)
            throw new PriceIsNotValidException();

        this.Quantity = quantity;
        this.Price = price;
        this.CustomerCode = customerCode;
        this.InstrumentCode = instrumentCode;
        this.OrderSide = orderSide;
    }

    internal Order(Guid id, string instrumentCode, OrderSide orderSide, int quantity, decimal price,
        string customerCode) : this(instrumentCode, orderSide, quantity, price, customerCode)
    {
        this.Id = id;
        this.CreatedOn = DateTime.Now;
        this.OrderState = OrderState.Active;
    }

    internal static IOrder New(IOrderOptions options)
    {
        return new Order(Guid.NewGuid(), options.InstrumentCode, options.OrderSide, options.Quantity, options.Price,
            options.CustomerCode);
    }

    internal static IOrderOptions NewOptions(IOrder order, int newQuantity, decimal newPrice)
    {
        return new Order(order.InstrumentCode, order.OrderSide, newQuantity, newPrice, order.CustomerCode);
    }

    public void SetLeftOver(int matchedQuantity)
    {
        this.Quantity -= matchedQuantity;
        this.ModifiedOn = DateTime.Now;
        SetAsFulfilled();
    }

    public void SetAsFulfilled()
    {
        if (this.Quantity == 0)
        {
            this.OrderState = OrderState.Fulfilled;
            this.ModifiedOn = DateTime.Now;
        }
    }

    public void SetAsCancelled()
    {
        this.OrderState = OrderState.Cancelled;
        this.ModifiedOn = DateTime.Now;
    }

    public bool IsValidToMatch()
    {
        return this.Quantity > 0 && this.OrderState == OrderState.Active;
    }

    public bool IsMatchedTo(IOrder otherOrder)
    {
        if (this.DoesBuyAndSellOrderBelongToACustomer(otherOrder)) return false;
        return this.OrderSide == OrderSide.Buy
            ? this.Price >= otherOrder.Price
            : this.Price <= otherOrder.Price;
    }

    private bool DoesBuyAndSellOrderBelongToACustomer(IOrder otherOrder)
    {
        return this.CustomerCode == otherOrder.CustomerCode;
    }
}