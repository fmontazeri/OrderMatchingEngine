using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.Tests.Unit.Consts;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

public class TestOrderBuilder
{
    public DateTime CreatedOn { get; private set; } = DateTime.Now;
    public DateTime ModifiedOn { get; private set; }
    public OrderSide OrderSide { get; private set; } = OrderSide.Buy;
    public int Quantity { get; private set; } = 100;
    public decimal Price { get; private set; } = 1000;
    public string CustomerCode { get; private set; } = CustomerConsts.FatemehMontazeri;
    public string InstrumentCode { get; private set; } = InstrumentConsts.OfoghBKoorosh;
    public OrderState OrderState { get; private set; } = OrderState.Active;

    public TestOrderBuilder WithCreatedOn(DateTime createdOn)
    {
        this.CreatedOn = createdOn;
        return this;
    }

    public TestOrderBuilder WithQuantity(int quantity)
    {
        this.Quantity = quantity;
        return this;
    }

    public TestOrderBuilder WithPrice(decimal price)
    {
        this.Price = price;
        return this;
    }

    public TestOrderBuilder WithOrderSide(OrderSide orderSide)
    {
        this.OrderSide = orderSide;
        return this;
    }

    public TestOrderBuilder WithCustomerCode(string customerCode)
    {
        this.CustomerCode = customerCode;
        return this;
    }

    public TestOrderBuilder WithInstrumentCode(string instrumentCode)
    {
        this.InstrumentCode = instrumentCode;
        return this;
    }

    public TestOrderBuilder WithModifiedOn(DateTime modifiedOn)
    {
        this.ModifiedOn = modifiedOn;
        return this;
    }
    public IOrder Build()
    {
        return new TestOrder(Guid.NewGuid(), this.InstrumentCode, this.Quantity, this.Price, this.OrderSide,
            this.CustomerCode, this.CreatedOn);
    }
    /// <summary>
    /// Has no default OrderId
    /// </summary>
    /// <returns></returns>
    public IOrder BuildOption()
    {
        return new TestOrder(this.InstrumentCode, this.Quantity, this.Price, this.OrderSide,
            this.CustomerCode, this.CreatedOn);
    }
}