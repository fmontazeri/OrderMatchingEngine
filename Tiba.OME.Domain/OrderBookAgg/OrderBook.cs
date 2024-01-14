using System.Collections;
using Tiba.OME.Domain.Core;
using Tiba.OME.Domain.OrderBookAgg.Comparers;
using Tiba.OME.Domain.OrderBookAgg.Events;
using Tiba.OME.Domain.OrderBookAgg.Exceptions;

namespace Tiba.OME.Domain.OrderBookAgg;

public class OrderBook : AggregateRootBase<Guid>
{
    public string InstrumentCode { get; private set; }
    public Hashtable Orders { get; } = new();

    private readonly PriorityQueue<IOrder, IOrder> _buyQueue;
    private readonly PriorityQueue<IOrder, IOrder> _sellQueue;

    public IEnumerable<IOrder?> GetOrders()
    {
        return from DictionaryEntry currentEngineOrder in this.Orders select (IOrder?)currentEngineOrder.Value;
    }

    public OrderBook(Guid id, string instrumentCode, List<IOrder> incomingOrders) :
        base(id)
    {
        _buyQueue = new PriorityQueue<IOrder, IOrder>(new BuyOrderComparer());
        _sellQueue = new PriorityQueue<IOrder, IOrder>(new SellOrderComparer());
        InstrumentCode = instrumentCode;

        foreach (var order in incomingOrders)
        {
            Orders.Add(order.Id, order);
            if (order.OrderSide == OrderSide.Buy)
            {
                _buyQueue.Enqueue(order, order);
            }
            else
            {
                _sellQueue.Enqueue(order, order);
            }
        }
    }

    public virtual IOrder AddOrder(IOrderOptions options)
    {
        GuardAgainstNotMatchInstrument(options);
        var incomingOrder = Order.New(options);
        Publish(new OrderPlacedDomainEvent(incomingOrder));
        Orders.Add(incomingOrder.Id, incomingOrder);
        MatchOrder(incomingOrder, incomingOrder.OrderSide == OrderSide.Buy ? _sellQueue : _buyQueue);
        return incomingOrder;
    }

    private void MatchOrder(IOrder incomingOrder, PriorityQueue<IOrder, IOrder> otherSideQueue)
    {
        IOrder? topOrder;
        while (otherSideQueue.Count > 0 && (topOrder = otherSideQueue.Peek()) != null &&
               !IsOrderFulfilled(incomingOrder))
        {
            if (!topOrder.IsValidToMatch())
            {
                otherSideQueue.Dequeue();
                continue;
            }

            if (!incomingOrder.IsMatchedTo(topOrder))
                break;

            var matchedQuantity = Math.Min(topOrder.Quantity, incomingOrder.Quantity);
            SetLeftOver(matchedQuantity, topOrder);
            SetLeftOver(matchedQuantity, incomingOrder);

            Publish(new OrderMatchedDomainEvent(matchedQuantity, incomingOrder, topOrder));
        }
    }

    public IOrder ModifyOrder(Guid orderId, int quantity, decimal price)
    {
        GuardAgainstInvalidOrder(orderId);
        var incomingOrder = (IOrder?)Orders[orderId];
        SetAsCancelled(incomingOrder);
        var options = Order.NewOptions(incomingOrder, quantity, price);
        return AddOrder(options);
    }

    public IOrder CancelOrder(Guid orderId)
    {
        var incomingOrder = (IOrder?)Orders[orderId];
        SetAsCancelled(incomingOrder);
        return incomingOrder;
    }

    private void GuardAgainstNotMatchInstrument(IOrderOptions options)
    {
        if (options.InstrumentCode != this.InstrumentCode)
            throw new InstrumentNotMatchException();
    }

    private void SetLeftOver(int matchedQuantity, IOrder order)
    {
        if (!Orders.ContainsKey(order.Id)) return;
        Orders.Remove(order.Id);
        order.SetLeftOver(matchedQuantity);
        if (order.IsValidToMatch())
            Orders.Add(order.Id, order);
    }

    private bool IsOrderFulfilled(IOrder incomingOrder)
    {
        return incomingOrder.Quantity == 0;
    }

    // private bool IsOrderActive(IOrder incomingOrder)
    // {
    //     return incomingOrder?.Quantity > 0;
    // }

    private void SetAsCancelled(IOrder? incomingOrder)
    {
        if (incomingOrder == null) throw new OrderNofFoundException();
        incomingOrder.SetAsCancelled();
        Publish(new OrderCancelledDomainEvent(incomingOrder));
        Orders.Remove(incomingOrder.Id);
    }

    private void GuardAgainstInvalidOrder(Guid orderId)
    {
        var incomingOrder = (IOrder?)Orders[orderId];
        if (incomingOrder == null) throw new OrderNofFoundException();
    }
}