using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Events;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class CancelOrder_EventTests : CancelOrder_StateTests
{
    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public override async Task CancelOrder_Should_Change_The_Given_OrderState_To_Cancelled(OrderSide orderSide)
    {
        await base.CancelOrder_Should_Change_The_Given_OrderState_To_Cancelled(orderSide);
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(1);
        CurrentOrderBook.AssertEvent(new OrderCancelledDomainEvent(PostedOrder));
    }
}