using FluentAssertions;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Comparers;
using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Builders;

namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.ComparerTests;

public class SellComparerTests
{
    private TestOrderBuilder _testOrderBuilder;

    public SellComparerTests()
    {
        _testOrderBuilder = new TestOrderBuilder();
    }

    [Theory]
    [InlineData(2000, 1000, 1)]
    [InlineData(1000, 2000, -1)]
    [InlineData(1000, 1000, -1)]
    public void SellComparerShouldCompareInThatWayTheMostCheapOrderBeAtTheTop(decimal firstPrice, decimal secondPrice,
        int expectedResult)
    {
        var comparer = new SellOrderComparer();
        var time = DateTime.Now;

        var order1 = _testOrderBuilder.WithPrice(firstPrice).WithCreatedOn(time).Build();
        var order2 = _testOrderBuilder.WithPrice(secondPrice).WithCreatedOn(time.AddSeconds(1)).Build();

        comparer.Compare(order1, order2).Should().Be(expectedResult);
    }


    [Fact]
    public void SellCompareShouldCompareInAWayThatTheMostExpensiveOrderBeAtTheTop()
    {
        var sellOrderQueue = new PriorityQueue<IOrder, IOrder>(new SellOrderComparer());
        var order1 = _testOrderBuilder.WithPrice(4000).Build();
        var order2 =_testOrderBuilder.WithPrice(1000).Build(); 
        var order3 =_testOrderBuilder.WithPrice(3000).Build(); 

        sellOrderQueue.Enqueue(order1, order1);
        sellOrderQueue.Enqueue(order2, order2);
        sellOrderQueue.Enqueue(order3, order3);
        var topOrder = sellOrderQueue.Peek();

        topOrder.Should().BeEquivalentTo(order2);
    }

    [Fact]
    public void SellComparerShouldCompareInThatWayTheFirstCheapOrderBeAtTheTopAmongOtherOrdersThatAreTheSamePrice()
    {
        var sellQueue = new PriorityQueue<IOrder, IOrder>(new SellOrderComparer());
        var time = DateTime.Now;

        var order1 = _testOrderBuilder.WithPrice(1000).WithCreatedOn(time.AddSeconds(3)).Build();
        var order2 = _testOrderBuilder.WithPrice(1000).WithCreatedOn(time).Build(); 

        sellQueue.Enqueue(order1, order1);
        sellQueue.Enqueue(order2, order2);
        var topOrder = sellQueue.Peek();

        topOrder.Should().BeEquivalentTo(order2);
    }
}