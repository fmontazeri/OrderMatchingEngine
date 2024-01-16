using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Exceptions;
using Tiba.OME.Domain.Tests.Unit.Consts;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class AddOrder_StateTests : BaseOrderBookTest
{
    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public virtual async Task AddOrder_Should_Place_Order_After_Receiving_The_First_Order(OrderSide orderSide)
    {
        CurrentOrderBook = _builder.Build();
        IncomingOrder = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        CurrentOrderBook.Orders.Count.Should().Be(1);
        AssertActiveOrder(order);
    }

    [Theory]
    [InlineData(OrderSide.Buy)]
    [InlineData(OrderSide.Sell)]
    public virtual async Task AddOrder_Should_Place_A_New_Order_When_Queue_Have_Some_Others(OrderSide orderSide)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(orderSide)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder.WithOrderSide(orderSide).BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        CurrentOrderBook.Orders.Count.Should().Be(2);
        AssertActiveOrder(order);
        AssertActiveOrder(PostedOrder);
    }

    [Theory]
    [InlineData(OrderSide.Sell, OrderSide.Buy)]
    [InlineData(OrderSide.Buy, OrderSide.Sell)]
    public virtual async Task AddOrder_IsMatched_When_The_BuyOrder_And_SellOrder_AreEqual(OrderSide queuedOrderSide,
        OrderSide incomingOrderSide)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(queuedOrderSide)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .WithQuantity(100)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(incomingOrderSide)
            .WithCustomerCode(CustomerConsts.YaserAbbasi)
            .WithQuantity(100)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        order.AssertFulfilledOrderState();
        CurrentOrderBook.Orders.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(1000, 1000)]
    [InlineData(11000, 1000)]
    public virtual async Task
        AddOrder_Should_Match_A_BuyOrder_When_The_Investor_IsWilling_ToPay_A_Price_At_Or_MoreExpensive_Than_ThePostedAsk(
            decimal bidPrice, decimal postedAskPrice)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Sell)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .WithPrice(postedAskPrice)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithCustomerCode(CustomerConsts.YaserAbbasi)
            .WithPrice(bidPrice)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        order.AssertFulfilledOrderState();
        CurrentOrderBook.Orders.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(1000, 1000)]
    [InlineData(900, 1000)]
    public virtual async Task
        AddOrder_Should_Match_A_SellOrder_When_The_Seller_IsWilling_ToOffer_A_Price_At_Or_Cheaper_Than_ThePostedBid(
            decimal askPrice, decimal postedBidPrice)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .WithPrice(postedBidPrice)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Sell)
            .WithCustomerCode(CustomerConsts.YaserAbbasi)
            .WithPrice(askPrice)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        order.AssertFulfilledOrderState();
        CurrentOrderBook.Orders.Count.Should().Be(0);
    }

    [Fact]
    public virtual async Task
        AddOrder_Should_Fulfill_A_SellOrder_When_The_AskQuantity_Is_Smaller_Than_ThePostedBid_With_TheSamePrice()
    {
        PostedOrder = NewOrder(OrderSide.Buy, 100, _testOrderBuilder.Price, CustomerConsts.FatemehMontazeri);
        EnqueueOrders(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Sell)
            .WithCustomerCode(CustomerConsts.YaserAbbasi)
            .WithQuantity(60)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        AssertFulfilledOrder(order);
        AssertPartiallyFulfilledOrder(PostedOrder, 40);
    }


    [Theory]
    [InlineData(OrderSide.Sell, 2500, OrderSide.Buy, 1300)]
    [InlineData(OrderSide.Buy, 900, OrderSide.Sell, 1000)]
    public virtual async Task
        AddOrder_Should_NotMatch_The_Given_Order_When_The_Buy_And_Sell_Orders_With_High_Priority_Are_Not_Compatible(
            OrderSide order1, decimal price1, OrderSide order2, decimal price2)
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(order1)
            .WithCustomerCode(CustomerConsts.YaserAbbasi)
            .WithPrice(price1)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(order2)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .WithPrice(price2)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        CurrentOrderBook.Orders.Count.Should().Be(2);
        CurrentOrderBook.Orders[order.Id].As<IOrder?>().AssertActiveOrderState();
        CurrentOrderBook.AssertOrderOptions(order);
        CurrentOrderBook.Orders[PostedOrder.Id].As<IOrder?>().AssertActiveOrderState();
        CurrentOrderBook.AssertOrderOptions(PostedOrder);
    }

    [Fact]
    public virtual async Task AddOrder_Should_Not_Be_Matched_When_Both_Sell_And_Buy_Orders_Are_From_The_Same_Customer()
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Sell)
            .WithCustomerCode(CustomerConsts.FatemehMontazeri)
            .BuildOption();

        var order = await CurrentOrderBook.AddOrder(IncomingOrder);

        CurrentOrderBook.Orders.Count.Should().Be(2);
        AssertActiveOrder(PostedOrder);
        AssertActiveOrder(order);
    }

    [Fact]
    public virtual async Task AddOrder_Should_Not_Matched_When_The_Both_Sell_And_Buy_Instrument_Are_Not_The_Same()
    {
        PostedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithInstrumentCode(InstrumentConsts.OfoghBKoorosh)
            .Build();
        CurrentOrderBook = _builder.Build(PostedOrder);
        IncomingOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Sell)
            .WithInstrumentCode(InstrumentConsts.ArvandPetro)
            .BuildOption();


        var exception = await Assert.ThrowsAsync<InstrumentNotMatchException>(async () =>
        {
            var order = await CurrentOrderBook.AddOrder(IncomingOrder);
        });
        exception.Message.Should().Be(InstrumentNotMatchException.ErrorMessage);
        CurrentOrderBook.Orders.Count.Should().Be(1);
        AssertActiveOrder(PostedOrder);
    }
}