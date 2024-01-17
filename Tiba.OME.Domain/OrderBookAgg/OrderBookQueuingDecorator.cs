using System.Collections.Concurrent;

namespace Tiba.OME.Domain.OrderBookAgg;

public class OrderBookQueuingDecorator : OrderBook, IAsyncDisposable, IDisposable
{
    private BlockingCollection<OrderQueueItem> _queue { get; } = new();

    private class OrderQueueItem(Func<Task<IOrder>> command)
    {
        public async Task Execute()
        {
            var res = await command();
            Completion.SetResult(res);
        }

        public TaskCompletionSource<IOrder> Completion { get; } = new();
    }

    private readonly Task processorTask;

    public OrderBookQueuingDecorator(Guid id, string instrumentCode, List<IOrder> orders) : base(id, instrumentCode,
        orders)
    {
        processorTask = Task.Run(() => Processor());
    }

    public override Task<IOrder> AddOrder(IOrderOptions options)
    {
        var item = new OrderQueueItem(() =>
        {
            var result = base.AddOrder(options);
            return result;
        });

        _queue.Add(item);
        return item.Completion.Task;
    }

    public override Task<IOrder> ModifyOrder(Guid orderId, int quantity, decimal price)
    {
        var item = new OrderQueueItem(() =>
        {
            var result = base.ModifyOrder(orderId, quantity, price);
            return result;
        });
        _queue.Add(item);
        return item.Completion.Task;
    }

    public override Task<IOrder> CancelOrder(Guid orderId)
    {
        var item = new OrderQueueItem(() =>
        {
            var result = base.CancelOrder(orderId);
            return result;
        });
        _queue.Add(item);
        return item.Completion.Task;
    }

    private async Task Processor()
    {
        while (!_queue.IsCompleted || _queue.Any())
        {
            OrderQueueItem? item = null;
            try
            {
                item = _queue.Take();
            }
            catch
            {
            }

            await item?.Execute();
        }
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        _queue.CompleteAdding();
        await processorTask;
    }
}