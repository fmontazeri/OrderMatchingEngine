using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tiba.OME.Application.CommandHandlers;
using Tiba.OME.Application.Contracts;
using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;
using Tiba.OME.Application.Exceptions;
using Tiba.OME.Application.Services;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Events;
using Tiba.OME.Domain.OrderBookAgg.Exceptions;
using Tiba.OME.Domain.Tests.Unit;
using Tiba.OME.Domain.Tests.Unit.Consts;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

namespace Tiba.OME.Application.Tests.Unit;

public class OrderBookTests : BaseOrderBookTest
{
    [Fact]
    public async Task AddOrder_Should_Place_IncomingOrder_In_Queue()
    {
        var orderBook =  _builder.Build();
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(orderBook.InstrumentCode).Returns(orderBook);
        var orderBookService = new OrderBookService(fakeOrderBookRepo);
        var command = new AddOrderCommand(OrderSide.Buy, 10, 1200, InstrumentConsts.OfoghBKoorosh,
            CustomerConsts.FatemehMontazeri);

        var expected = await orderBookService.AddOrder(command);

        await fakeOrderBookRepo.Received(1).UpdateAsync(orderBook);
        await fakeOrderBookRepo.Received(1).GetByInstrumentCode(orderBook.InstrumentCode);
        orderBook.Orders.Count.Should().Be(1);
        orderBook.AssertOrderOptions(expected);
        orderBook.GetPublishedEvents().Should().HaveCount(1);
        orderBook.AssertEvent(new OrderPlacedDomainEvent(expected));
    }

    [Fact]
    public async Task UpdateOrder_Should_Cancel_The_GivenOrder_And_Then_Place_A_New_Order_By_NewPrice_In_Queue()
    {
        var postedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithPrice(1200)
            .Build();
        var orderBook = _builder.Build(postedOrder);
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(postedOrder.InstrumentCode).Returns(orderBook);
        var fakeOrderBookService = new OrderBookService(fakeOrderBookRepo);
        var command = new UpdateOrderCommand(postedOrder.Id, postedOrder.Quantity, 1600, postedOrder.InstrumentCode);

        var expected = await fakeOrderBookService.UpdateOrder(command);

        await fakeOrderBookRepo.Received(1).UpdateAsync(orderBook);
        await fakeOrderBookRepo.Received(1).GetByInstrumentCode(postedOrder.InstrumentCode);
        postedOrder.OrderState.Should().Be(OrderState.Cancelled);
        orderBook.Orders.Count.Should().Be(1);
        orderBook.Orders[postedOrder.Id].Should().BeNull();
        orderBook.AssertOrderOptions(expected);
        orderBook.GetPublishedEvents().Should().HaveCount(2);
        orderBook.AssertEvent(new OrderCancelledDomainEvent(postedOrder));
        orderBook.AssertEvent(new OrderPlacedDomainEvent(expected));
    }


    [Theory]
    [InlineData(100, 0)]
    [InlineData(100, -10)]
    public async Task UpdateOrder_Should_Throw_Exception_Where_The_New_Quantity_Is_Invalid(
        int quantity, int newQuantity)
    {
        var postedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithQuantity(quantity)
            .Build();
        var orderBook = _builder.Build(postedOrder);
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(postedOrder.InstrumentCode).Returns(orderBook);
        var orderBookService = new OrderBookService(fakeOrderBookRepo);
        var command =
            new UpdateOrderCommand(postedOrder.Id, newQuantity, postedOrder.Price, postedOrder.InstrumentCode);

        var exception = await Assert.ThrowsAsync<QuantityIsNotValidException>(async () =>
        {
            await orderBookService.UpdateOrder(command);
        });
        exception.Message.Should().Be(QuantityIsNotValidException.ErrorMessage);
    }


    [Theory]
    [InlineData(100, 0)]
    [InlineData(100, -10)]
    public async Task UpdateOrder_Should_Throw_Exception_Where_The_New_Quantity_Is_Lower_Than_The_Given_Price(
        decimal price, decimal newPrice)
    {
        var postedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .WithPrice(price)
            .Build();
        var orderBook = _builder.Build(postedOrder);
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(postedOrder.InstrumentCode).Returns(orderBook);
        var orderBookService = new OrderBookService(fakeOrderBookRepo);
        var command =
            new UpdateOrderCommand(postedOrder.Id, postedOrder.Quantity, newPrice, postedOrder.InstrumentCode);

        var exception = await Assert.ThrowsAsync<PriceIsNotValidException>(async () =>
        {
            await orderBookService.UpdateOrder(command);
        });
        exception.Message.Should().Be(PriceIsNotValidException.ErrorMessage);
    }

    [Fact]
    public async Task CancelOrder_Should_Cancel_The_Given_Order_By_Removing_It_From_The_Queue()
    {
        var postedOrder = _testOrderBuilder
            .WithOrderSide(OrderSide.Buy)
            .Build();
        var orderBook = _builder.Build(postedOrder);
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(postedOrder.InstrumentCode).Returns(orderBook);
        var orderBookService = new OrderBookService(fakeOrderBookRepo);
        var cancelOrderCommand = new CancelOrderCommand(postedOrder.Id, postedOrder.InstrumentCode);

        var expected = await orderBookService.CancelOrder(cancelOrderCommand);

        await fakeOrderBookRepo.Received(1).UpdateAsync(orderBook);
        expected.OrderState.Should().Be(OrderState.Cancelled);
        orderBook.Orders.Count.Should().Be(0);
        orderBook.Orders[postedOrder.Id].Should().BeNull();
        orderBook.GetPublishedEvents().Count.Should().Be(1);
        orderBook.AssertEvent(new OrderCancelledDomainEvent(expected));
    }

    [Fact]
    public async Task AddOrder_Should_Throw_Exception_When_The_Instrument_Is_NotValid()
    {
        var fakeOrderBookRepo = Substitute.For<IOrderBookRepository>();
        fakeOrderBookRepo.GetByInstrumentCode(Arg.Any<string>()).ReturnsNull();
        var orderBookService = new OrderBookService(fakeOrderBookRepo);
        var command = new AddOrderCommand(OrderSide.Buy, 10, 1200, InstrumentConsts.OfoghBKoorosh,
            CustomerConsts.FatemehMontazeri);

        var exception = await Assert.ThrowsAsync<OrderBookNotFound>(async () =>
        {
            await orderBookService.AddOrder(command);
        });
        exception.Message.Should().Be(OrderBookNotFound.ErrorMessage);
    }
}