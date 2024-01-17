using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Exceptions;
using Tiba.OME.Domain.Tests.Unit.Consts;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;

public class ModifyOrder_StateTests : BaseOrderBookTest
{
    [Fact]
    public virtual async Task
        ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_Investor_Change_TheQuantity()
    {
        var price = 1000;
        PostedOrder = NewOrder(OrderSide.Buy, 10, price);
        EnqueueOrders(PostedOrder);

        var newQuantity = 13;
        ModifiedOrder = await CurrentOrderBook.ModifyOrder(PostedOrder.Id, newQuantity, price);

        AssertCancelledOrder(PostedOrder);
        AssertActiveOrder(ModifiedOrder);
    }


    [Fact]
    public virtual async Task
        ModifyOrder_Should_Cancel_The_Given_BuyOrder_And_Then_Place_A_NewBuyOrder_When_The_Investor_Change_The_Price()
    {
        var quantity = 10;
        PostedOrder = NewOrder(OrderSide.Buy, quantity, 5000);
        EnqueueOrders(PostedOrder);

        var newPrice = 8000;
        ModifiedOrder = await CurrentOrderBook.ModifyOrder(PostedOrder.Id, quantity, newPrice);

        CurrentOrderBook.Orders.Count.Should().Be(1);
        AssertCancelledOrder(PostedOrder);
        AssertActiveOrder(ModifiedOrder);
    }

    [Fact]
    public virtual async Task
        ModifyOrder_Fulfill_The_Given_BuyOrder_When_ItsQuantity_Modify_To_TheFirst_Available_PostedAsk_Quantity()
    {
        PostedOrder = NewOrder(OrderSide.Buy, 10, 1000);
        var sellOrder = NewOrder(OrderSide.Sell, 8, 2000, CustomerConsts.FatemehMontazeri);
        EnqueueOrders(PostedOrder, sellOrder);

        ModifiedOrder = await CurrentOrderBook.ModifyOrder(PostedOrder.Id, 8, 2100);

        CurrentOrderBook.Orders.Count.Should().Be(0);
        AssertCancelledOrder(PostedOrder);
        AssertFulfilledOrder(sellOrder);
    }


    [Fact]
    public virtual async Task
        ModifyOrder_Should_Partially_Fill_The_Given_PostedBid_When_ItsQuantity_Modified_To_TheNumber_Greater_Than_PostedAsk_Quantity()
    {
        PostedOrder = NewOrder(OrderSide.Buy, 10, 1000);
        var sellOrder = NewOrder(OrderSide.Sell, 8, 2000, CustomerConsts.FatemehMontazeri);
        EnqueueOrders(PostedOrder, sellOrder);

        ModifiedOrder = await CurrentOrderBook.ModifyOrder(PostedOrder.Id, 15, 2100);

        CurrentOrderBook.Orders.Count.Should().Be(1);
        AssertFulfilledOrder(sellOrder);
        AssertPartiallyFulfilledOrder(ModifiedOrder, 7);
    }


    [Fact]
    public async Task ModifyOrder_Should_Throw_Exception_When_Order_NotFound()
    {
        var currentOrderBook = _builder.Build();
        var notExistOrderId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<OrderNofFoundException>(async () =>
        {
            await currentOrderBook.ModifyOrder(notExistOrderId, 10, 13);
        });
        
        exception.Message.Should().Be(OrderNofFoundException.ErrorMessage);
    }
}