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
        var IncomingOrder = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();

        var order = await queuingDecorator.AddOrder(IncomingOrder);

        queuingDecorator.Orders.Count.Should().Be(1);
        AssertActiveOrder(order,queuingDecorator);
    }

    public void AssertActiveOrder(IOrder order , OrderBookQueuingDecorator orderBook)
    {
        orderBook.Orders[order.Id].As<IOrder?>().AssertActiveOrderState();
        orderBook.AssertOrderOptions(order);
    }
}