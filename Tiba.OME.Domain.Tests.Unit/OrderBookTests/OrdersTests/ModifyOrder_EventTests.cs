using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Events;
using Tiba.OME.Domain.Tests.Unit.Consts;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class ModifyOrder_EventTests : ModifyOrder_StateTests
{
    [Fact]
    public override async Task
        ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_Investor_Change_TheQuantity()
    {
        await base
            .ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_Investor_Change_TheQuantity();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(2);
        CurrentOrderBook.AssertEvent(new OrderCancelledDomainEvent(PostedOrder));
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(ModifiedOrder));
    }

    [Fact]
    public override async Task
        ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_The_Investor_Change_The_Price()
    {
        await base
            .ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_The_Investor_Change_The_Price();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(2);
        CurrentOrderBook.AssertEvent(new OrderCancelledDomainEvent(PostedOrder));
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(ModifiedOrder));
    }

    [Fact]
    public override async Task
        ModifyOrder_Fulfill_The_Given_BuyOrder_When_ItsQuantity_Modify_To_TheFirst_Available_PostedAsk_Quantity()
    {
        await base
            .ModifyOrder_Fulfill_The_Given_BuyOrder_When_ItsQuantity_Modify_To_TheFirst_Available_PostedAsk_Quantity();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(3);
        var matchedQuantity = 8;
        CurrentOrderBook.AssertEvent(new OrderCancelledDomainEvent(PostedOrder));
        var actualBid = NewOrder(OrderSide.Buy, matchedQuantity, 2100);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(actualBid));
        var actualAsk = NewOrder(OrderSide.Sell, matchedQuantity, 2000, CustomerConsts.FatemehMontazeri);
        CurrentOrderBook.AssertEvent(new OrderMatchedDomainEvent(matchedQuantity, actualBid, actualAsk));
    }

    [Fact]
    public override async Task
        ModifyOrder_Should_Partially_Fill_The_Given_PostedBid_When_ItsQuantity_Modified_To_TheNumber_Greater_Than_PostedAsk_Quantity()
    {
        await base
            .ModifyOrder_Should_Partially_Fill_The_Given_PostedBid_When_ItsQuantity_Modified_To_TheNumber_Greater_Than_PostedAsk_Quantity();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(3);
        CurrentOrderBook.AssertEvent(new OrderCancelledDomainEvent(PostedOrder));
        var actualBid = NewOrder(OrderSide.Buy, 15, 2100);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(actualBid));
        var actualAsk = NewOrder(OrderSide.Buy, 8, 2000, CustomerConsts.FatemehMontazeri);
        CurrentOrderBook.AssertEvent(new OrderMatchedDomainEvent(8, PostedOrder, actualAsk));
    }
}