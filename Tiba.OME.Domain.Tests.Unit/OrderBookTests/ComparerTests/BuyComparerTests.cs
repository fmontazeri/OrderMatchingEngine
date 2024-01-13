using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Comparers;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.ComparerTests;

public class BuyComparerTests
{

    private TestOrderBuilder _testOrderBuilder;

    public BuyComparerTests()
    {
        _testOrderBuilder = new TestOrderBuilder();
    }
    [Theory]
    [InlineData(2000, 1000, -1)]
    [InlineData(1000, 2000, 1)]
    [InlineData(1000, 1000, -1)]
    public void BuyComparerShouldCompareInThatWayTheMostCheapOrderBeAtTheTop(decimal firstOrderPrice, decimal secondOrderPrice, int expectedResult)
    {
        var comparer = new BuyOrderComparer();
        var time = DateTime.Now;
        var order1 = _testOrderBuilder.WithPrice(firstOrderPrice).WithCreatedOn(time).Build();
        var order2 = _testOrderBuilder.WithPrice(secondOrderPrice).WithCreatedOn(time.AddSeconds(10)).Build();

        comparer.Compare(order1, order2).Should().Be(expectedResult);
    }


    [Fact]
    public void BuyCompareShouldCompareInAWayThatTheEarliestOrderBeAtTheTopAmongOtherOrdersThatAreTheSamePriceLevel()
    {
        var buyOrderQueue = new PriorityQueue<IOrder, IOrder>(new BuyOrderComparer());
        var time = DateTime.Now;
        var order1 = _testOrderBuilder.WithPrice(1000).WithCreatedOn(time.AddSeconds(1)).Build();
        var order2 = _testOrderBuilder.WithPrice(1000).WithCreatedOn(time).Build();

        buyOrderQueue.Enqueue(order1, order1);
        buyOrderQueue.Enqueue(order2, order2);
        var topOrder = buyOrderQueue.Peek();

        topOrder.Should().BeEquivalentTo(order2);
    }
}