using FluentAssertions;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests;

public partial class OrderBookTests : BaseOrderBookTest
{
    [Fact]
    public void OrderBook_Should_Construct_When_No_Orders_Received()
    {
        var currentEngine = _builder.Build();

        currentEngine.InstrumentCode.Should().Be(_builder.InstrumentCode);
        currentEngine.Orders.Count.Should().Be(0);
    }

    [Fact]
    public void Create_Should_Be_Constructed_When_Some_Orders_Received()
    {
        //Arrange
        var order = _testOrderBuilder.Build();

        //Act
        var currentEngine = _builder.Build(order);

        //Assert
        currentEngine.InstrumentCode.Should().Be(_builder.InstrumentCode);
        currentEngine.Orders.Count.Should().Be(1);
        currentEngine.Orders[order.Id].Should().NotBeNull();
        currentEngine.AssertOrderOptions(order);
    }
}