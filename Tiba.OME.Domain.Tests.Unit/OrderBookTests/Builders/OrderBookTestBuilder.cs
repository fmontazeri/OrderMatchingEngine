using NSubstitute;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.Tests.Unit.Consts;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

public class OrderBookTestBuilder
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string InstrumentCode { get; private set; } = InstrumentConsts.OfoghBKoorosh;

    public List<IOrder> Orders { get; private set; } = new();

    public OrderBookTestBuilder WithInstrumentCode(string instrumentCode)
    {
        this.InstrumentCode = instrumentCode;
        return this;
    }

    public OrderBook Build(params IOrder[] orders)
    {
        this.Orders.AddRange(orders);
        return new OrderBook(this.Id, InstrumentCode, Orders);
    }

    public OrderBook Build()
    {
      //  return new OrderBook(this.Id, InstrumentCode, Orders);
      return new OrderBookQueuingDecorator(this.Id, InstrumentCode, Orders);
    }
    
    public OrderBookQueuingDecorator BuildWithDecorator()
    {
        return new OrderBookQueuingDecorator(this.Id, InstrumentCode, Orders);
    }
}