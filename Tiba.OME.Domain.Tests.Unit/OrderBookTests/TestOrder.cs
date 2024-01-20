using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests;

public class TestOrder(
    string instrumentCode,
    int quantity,
    decimal price,
    OrderSide orderSide,
    string customerCode,
    DateTime createdOn)
    : IOrder
{
    public Guid Id { get; private set; }
    public DateTime CreatedOn { get; set; } = createdOn;
    public DateTime ModifiedOn { get; set; }
    public OrderSide OrderSide { get; set; } = orderSide;
    public int Quantity { get; private set; } = quantity;
    public decimal Price { get; private set; } = price;
    public string CustomerCode { get; private set; } = customerCode;
    public string InstrumentCode { get; private set; } = instrumentCode;
    public OrderState OrderState { get; private set; } = OrderState.Active;


    public TestOrder(Guid id, string instrumentCode, int quantity, decimal price, OrderSide orderSide,
        string customerCode, DateTime createdOn) :
        this(instrumentCode, quantity, price, orderSide, customerCode, createdOn)
    {
        Id = id;
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

    public bool IsOrderFulfilled()
    {
        return this.Quantity == 0 && this.OrderState == OrderState.Fulfilled;
    }

    private bool DoesBuyAndSellOrderBelongToACustomer(IOrder otherOrder)
    {
        return this.CustomerCode == otherOrder.CustomerCode;
    }
}