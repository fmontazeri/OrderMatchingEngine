using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Events;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class AddOrder_EventTests : AddOrder_StateTests
{
    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public override void AddOrder_Should_Place_Order_After_Receiving_The_First_Order(OrderSide orderSide)
    {
        base.AddOrder_Should_Place_Order_After_Receiving_The_First_Order(orderSide);
        AssertOrderPlacedEvent();
    }

    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public override void AddOrder_Should_Place_A_New_Order_When_Queue_Have_Some_Others(OrderSide orderSide)
    {
        base.AddOrder_Should_Place_A_New_Order_When_Queue_Have_Some_Others(orderSide);
        AssertOrderPlacedEvent();
    }

    private void AssertOrderPlacedEvent()
    {
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(1);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(IncomingOrder));
    }

    [Theory]
    [InlineData(OrderSide.Sell, OrderSide.Buy)]
    [InlineData(OrderSide.Buy, OrderSide.Sell)]
    public override void AddOrder_IsMatched_When_The_BuyOrder_And_SellOrder_AreEqual(OrderSide queuedOrderSide,
        OrderSide incomingOrderSide)
    {
        base.AddOrder_IsMatched_When_The_BuyOrder_And_SellOrder_AreEqual(queuedOrderSide, incomingOrderSide);
        AssertMatchedOrderEvent(100);
    }

    private void AssertMatchedOrderEvent(int matchedQuantity)
    {
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(2);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(IncomingOrder));
        CurrentOrderBook.AssertEvent(new OrderMatchedDomainEvent(matchedQuantity, IncomingOrder, PostedOrder));
    }

    [Theory]
    [InlineData(1000, 1000)]
    [InlineData(11000, 1000)]
    public override void
        AddOrder_Should_Match_A_BuyOrder_When_The_Investor_IsWilling_ToPay_A_Price_At_Or_MoreExpensive_Than_ThePostedAsk(
            decimal buyPrice, decimal postedAskPrice)
    {
        base
            .AddOrder_Should_Match_A_BuyOrder_When_The_Investor_IsWilling_ToPay_A_Price_At_Or_MoreExpensive_Than_ThePostedAsk(
                buyPrice, postedAskPrice);
        AssertMatchedOrderEvent(_testOrderBuilder.Quantity);
    }

    [Theory]
    [InlineData(1000, 1000)]
    [InlineData(900, 1000)]
    public override void
        AddOrder_Should_Match_A_SellOrder_When_The_Seller_IsWilling_ToOffer_A_Price_At_Or_Cheaper_Than_ThePostedBid(
            decimal askPrice, decimal postedBidPrice)
    {
        base
            .AddOrder_Should_Match_A_SellOrder_When_The_Seller_IsWilling_ToOffer_A_Price_At_Or_Cheaper_Than_ThePostedBid(
                askPrice, postedBidPrice);
        AssertMatchedOrderEvent(_testOrderBuilder.Quantity);
    }

    [Fact]
    public override void
        AddOrder_Should_Fulfill_A_SellOrder_When_The_AskQuantity_Is_Smaller_Than_ThePostedBid_With_TheSamePrice()
    {
        base.AddOrder_Should_Fulfill_A_SellOrder_When_The_AskQuantity_Is_Smaller_Than_ThePostedBid_With_TheSamePrice();
        AssertMatchedOrderEvent(60);
    }

    [Theory]
    [InlineData(OrderSide.Sell, 2500, OrderSide.Buy, 1300)]
    [InlineData(OrderSide.Buy, 900, OrderSide.Sell, 1000)]
    public override void
        AddOrder_Should_NotMatch_The_Given_Order_When_The_Buy_And_Sell_Orders_With_High_Priority_Are_Not_Compatible(
            OrderSide order1, decimal price1, OrderSide order2, decimal price2)
    {
        base
            .AddOrder_Should_NotMatch_The_Given_Order_When_The_Buy_And_Sell_Orders_With_High_Priority_Are_Not_Compatible(
                order1, price1, order2, price2);
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(1);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(IncomingOrder));
    }


    [Fact]
    public override void AddOrder_Should_Not_Be_Matched_When_Both_Sell_And_Buy_Orders_Are_From_The_Same_Customer()
    {
        base.AddOrder_Should_Not_Be_Matched_When_Both_Sell_And_Buy_Orders_Are_From_The_Same_Customer();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(1);
        CurrentOrderBook.AssertEvent(new OrderPlacedDomainEvent(IncomingOrder));
    }

    [Fact]
    public override void AddOrder_Should_Not_Matched_When_The_Both_Sell_And_Buy_Instrument_Are_Not_The_Same()
    {
        base.AddOrder_Should_Not_Matched_When_The_Both_Sell_And_Buy_Instrument_Are_Not_The_Same();
        CurrentOrderBook.GetPublishedEvents().Should().HaveCount(0);
    }
}