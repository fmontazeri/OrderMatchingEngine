using FluentAssertions;
using Tiba.OME.Domain.Core;
using Tiba.OME.Domain.OrderBookAgg;
using Tiba.OME.Domain.OrderBookAgg.Events;

namespace Tiba.OME.Domain.Tests.Unit;

public static class FluentAssertionsExtensions
{
    public static void ShouldContainsEquivalencyOfDomainEvent<TKey, TExpectation>(
        this AggregateRootBase<TKey> aggregate,
        TExpectation expectation,
        string? because = null,
        params object[] becauseArgs)
        where TExpectation : IEvent
        where TKey : new()
    {
        //aggregate.GetPublishedEvents().First(z => z.GetType() == expectation.GetType()).Should().NotBeNull();
        aggregate.GetPublishedEvents()
            .Should()
            .ContainEquivalentOf(expectation, opt => opt
                    .Excluding(a => a.PublishDateTime)
                    .Excluding(a => a.EventId)
                , because, becauseArgs);
    }

    public static void ShouldContainsEquivalencyOfDomainEvent<TKey, TExpectation>(
        this AggregateRootBase<TKey> aggregate,
        TExpectation[] expectations,
        string because = "",
        params object[] becauseArgs) where TExpectation : DomainEvent
    {
        foreach (var expectation in expectations)
        {
            aggregate.GetPublishedEvents().Should().ContainEquivalentOf(
                expectation, opt => opt
                    .Excluding(x => x.Path.StartsWith("EventId"))
                    .Excluding(x => x.Path.StartsWith("PublishDateTime"))
                , because, becauseArgs);
        }
    }

    // public static void AssertOrder(this MatchingEngine engine, IOrder expected)
    // {
    //     foreach (DictionaryEntry entry in engine.Orders)
    //     {
    //         var order = (IOrder?)entry.Value;
    //         order.Should().NotBeNull();
    //         order.Id.Should().NotBeEmpty();
    //         order.Should().BeEquivalentTo(expected, options => options
    //             .Excluding(ex => ex.Id)
    //             .Excluding(ex => ex.CreatedOn)
    //             .Excluding(ex => ex.ModifiedOn));
    //         break;
    //     }
    // }

    public static void AssertOrderOptions(this OrderBook engine, IOrder expected)
    {
        engine.Orders[expected.Id].Should().NotBeNull();
        engine.Orders[expected.Id].Should().BeEquivalentTo(expected, options => options.Excluding(ex => ex.CreatedOn));
    }


    public static void AssertActiveOrderState(this IOrder? order)
    {
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
        order.Quantity.Should().BeGreaterThan(0);
        order.OrderState.Should().Be(OrderState.Active);
    }

    public static void AssertFulfilledOrderState(this IOrder? order)
    {
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
        order.Quantity.Should().Be(0);
        order.OrderState.Should().Be(OrderState.Fulfilled);
    }

    public static void AssertEvent(this OrderBook engine, OrderPlacedDomainEvent expected)
    {
        var orderPlacedEvent = engine.GetPublishedEvents().OfType<OrderPlacedDomainEvent>().First();
        orderPlacedEvent.Should().BeEquivalentTo(expected, options => options
            .Excluding(ex => ex.Path.StartsWith("OrderId"))
            .Excluding(ex => ex.EventId)
            .Excluding(ex => ex.PublishDateTime));
    }

    public static void AssertEvent(this OrderBook engine, OrderMatchedDomainEvent expected)
    {
        var orderPlacedEvent = engine.GetPublishedEvents().OfType<OrderMatchedDomainEvent>().First();
        orderPlacedEvent.Should().BeEquivalentTo(expected, options => options
            .Excluding(ex => ex.Path.StartsWith("BuyOrderId"))
            .Excluding(ex => ex.Path.StartsWith("SellOrderId"))
            .Excluding(ex => ex.EventId)
            .Excluding(ex => ex.PublishDateTime));
    }

    public static void AssertEvent(this OrderBook engine, OrderCancelledDomainEvent expected)
    {
        var orderPlacedEvent = engine.GetPublishedEvents().OfType<OrderCancelledDomainEvent>().First();
        orderPlacedEvent.Should().BeEquivalentTo(expected, options => options
            .Excluding(ex => ex.Path.StartsWith("OrderId"))
            .Excluding(ex => ex.EventId)
            .Excluding(ex => ex.PublishDateTime));
    }

    // public static void AssertEvent<T>(this MatchingEngine engine, params T[] expectedEvents) where T : IEvent
    // {
    //     foreach (var expected in expectedEvents)
    //     {
    //        var @event = engine.GetPublishedEvents().OfType<T>().First();
    //         
    //         engine.GetPublishedEvents().Should().ContainEquivalentOf(expected, options => options
    //             .Excluding(ex => ex.Path.StartsWith("OrderId"))
    //             .Excluding(ex => ex.Path.StartsWith("EventId"))
    //             .Excluding(ex => ex.Path.StartsWith("PublishDateTime"))
    //             .Excluding(ex => ex.Path.StartsWith("SellerId"))
    //             .Excluding(ex => ex.Path.StartsWith("BuyerId")));
    //         
    //         // @event.Should().BeEquivalentTo(expected, options => options
    //         //     .Excluding(ex => ex.Path.StartsWith("OrderId"))
    //         //     .Excluding(ex => ex.Path.StartsWith("EventId"))
    //         //     .Excluding(ex => ex.Path.StartsWith("PublishDateTime")));
    //     }
    // }
}