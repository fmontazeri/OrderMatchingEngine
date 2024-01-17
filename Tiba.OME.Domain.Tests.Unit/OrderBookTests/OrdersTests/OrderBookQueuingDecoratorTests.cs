using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class OrderBookQueuingDecoratorTests
{
    private OrderBookTestBuilder _builder;
    private TestOrderBuilder _testOrderBuilder;

    public OrderBookQueuingDecoratorTests()
    {
        _builder = new OrderBookTestBuilder();
        _testOrderBuilder = new TestOrderBuilder();
    }

    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public virtual async Task AddOrder_Should_Place_Order_After_Receiving_The_First_Order(OrderSide orderSide)
    {
        await using var queuingDecorator = _builder.BuildWithDecorator();
        var IncomingOrder1 = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();
        var IncomingOrder2 = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();
        var IncomingOrder3 = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();

        var order1 = await queuingDecorator.AddOrder(IncomingOrder1);
        var order2 = await queuingDecorator.AddOrder(IncomingOrder2);
        var order3 = await queuingDecorator.AddOrder(IncomingOrder3);

        queuingDecorator.Orders.Count.Should().Be(3);
       // AssertActiveOrder(order, queuingDecorator);
    }

    public void AssertActiveOrder(IOrder order, OrderBookQueuingDecorator orderBook)
    {
        orderBook.Orders[order.Id].As<IOrder?>().AssertActiveOrderState();
        orderBook.AssertOrderOptions(order);
    }
}