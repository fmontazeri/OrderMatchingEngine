using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests;

public class TestOrder : IOrder
{
    public Guid Id { get; private set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public OrderSide OrderSide { get; set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public string CustomerCode { get; private set; }
    public string InstrumentCode { get; private set; }
    public OrderState OrderState { get; private set; }


    public TestOrder( Guid id, string instrumentCode, int quantity, decimal price, OrderSide orderSide, string customerCode , DateTime createdOn) 
    {
        Id = id;
        OrderSide = orderSide;
        Quantity = quantity;
        Price = price;
        CustomerCode = customerCode;
        InstrumentCode = instrumentCode;
        CreatedOn = createdOn;
        OrderState = OrderState.Active;
    }
    
    public TestOrder(string instrumentCode, int quantity, decimal price, OrderSide orderSide, string customerCode , DateTime createdOn) 
    {
        OrderSide = orderSide;
        Quantity = quantity;
        Price = price;
        CustomerCode = customerCode;
        InstrumentCode = instrumentCode;
        CreatedOn = createdOn;
        OrderState = OrderState.Active;
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
}