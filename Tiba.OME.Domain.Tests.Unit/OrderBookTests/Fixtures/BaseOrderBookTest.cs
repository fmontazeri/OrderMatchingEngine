using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.Tests.Unit.Consts;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

public abstract class BaseOrderBookTest
{
    public readonly OrderBookTestBuilder _builder = new();

    public readonly TestOrderBuilder _testOrderBuilder = new();

    // public OrderBookQueuingDecorator CurrentOrderBook { get; set; }
    public OrderBook CurrentOrderBook { get; set; }
    public IOrder PostedOrder { get; set; }
    public IOrder IncomingOrder { get; set; }
    public IOrder ModifiedOrder { get; set; }

    // protected async  Task EnqueueOrders(params IOrder[] incomingOrders)
    // {
    //     await using var queuingDecorator = _builder.Build(incomingOrders);
    //     CurrentOrderBook = queuingDecorator;
    //     //CurrentOrderBook =  _builder.Build(incomingOrders);
    // }
    //

    protected void EnqueueOrders(params IOrder[] incomingOrders)
    {
        CurrentOrderBook = _builder.Build(incomingOrders);
    }

    protected IOrder NewOrder(OrderSide orderSide, int quantity, decimal price,
        string customerCode = CustomerConsts.YaserAbbasi)
    {
        var order1 = _testOrderBuilder
            .WithOrderSide(orderSide)
            .WithCustomerCode(customerCode)
            .WithQuantity(quantity)
            .WithPrice(price)
            .Build();
        return order1;
    }

    protected void AssertActiveOrder(IOrder order)
    {
        CurrentOrderBook.Orders[order.Id].As<IOrder?>().AssertActiveOrderState();
        CurrentOrderBook.AssertOrderOptions(order);
    }

    protected void AssertCancelledOrder(IOrder order)
    {
        order.OrderState.Should().Be(OrderState.Cancelled);
        CurrentOrderBook.Orders[order.Id].Should().BeNull();
    }

    protected void AssertFulfilledOrder(IOrder order)
    {
        order.AssertFulfilledOrderState();
        CurrentOrderBook.Orders[order.Id].As<IOrder?>().Should().BeNull();
    }

    protected void AssertPartiallyFulfilledOrder(IOrder order, int expectedQuantity)
    {
        AssertActiveOrder(order);
        order.Quantity.Should().Be(expectedQuantity);
    }
}