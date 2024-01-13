using System.Collections.Concurrent;

namespace Tiba.OME.Application.Services;

public class OrderBookCollectionQueue
{
    private BlockingCollection<object> _orders = new();

    public OrderBookCollectionQueue()
    {
        var thread = new Thread(OnStart)
        {
            IsBackground = true
        };
        thread.Start();
    }

    public void Enqueue(params object[] orders)
    {
        foreach (var order in orders)
        {
            _orders.Add(order);
        }
    }

    private void OnStart()
    {
        foreach (var order in _orders.GetConsumingEnumerable(CancellationToken.None))
        {
            Console.Write("--------");
        }
    }
}