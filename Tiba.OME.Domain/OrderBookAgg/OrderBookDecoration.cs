using System.Collections.Concurrent;

namespace Tiba.OME.Domain.OrderBookAgg;

public class DomainHandlerDecorator : OrderBook
{
    private BlockingCollection<(Func<Task<IOrder>> action, TaskCompletionSource<IOrder> tcs)> _queue { get; } = new();

    private readonly Task _processItemsTask;

    public DomainHandlerDecorator(Guid id, string instrumentCode, List<IOrder> orders) : base(id, instrumentCode,
        orders)
    {
        Processor().Start();
    }

    public override Task<IOrder> AddOrder(IOrderOptions options)
    {
        var taskCompletionSource = new TaskCompletionSource<IOrder>();
        _queue.Add((() =>
        {
            var result = base.AddOrder(options);
            return result;
        }, taskCompletionSource));
        return taskCompletionSource.Task;
    }

    public override Task<IOrder> ModifyOrder(Guid orderId, int quantity, decimal price)
    {
        var taskCompletionSource = new TaskCompletionSource<IOrder>();
        _queue.Add((() =>
        {
            var result = base.ModifyOrder(orderId, quantity, price);
            return result;
        }, taskCompletionSource));
        return taskCompletionSource.Task;
    }

    public override Task<IOrder> CancelOrder(Guid orderId)
    {
        var taskCompletionSource = new TaskCompletionSource<IOrder>();
        _queue.Add((() =>
        {
            var result = base.CancelOrder(orderId);
            return result;
        }, taskCompletionSource));
        return taskCompletionSource.Task;
    }

    private async Task Processor()
    {
        foreach (var orderTask in _queue.GetConsumingEnumerable(CancellationToken.None))
        {
            var result = await orderTask.action.Invoke();
            orderTask.tcs.SetResult(result);
        }
    }
}