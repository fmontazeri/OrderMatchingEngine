using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class CancelOrder_StateTests : BaseOrderBookTest
{
    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public virtual async Task CancelOrder_Should_Change_The_Given_OrderState_To_Cancelled(OrderSide orderSide)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(orderSide)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);

        var order = await CurrentOrderBook.CancelOrder(PostedOrder.Id);

        CurrentOrderBook.Orders.Count.Should().Be(0);
        order.OrderState.Should().Be(OrderState.Cancelled);
    }
}