using Tiba.OME.Application.Contracts.Services;
using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Services;

public interface IQueueOrderDecorator
{
    Task EnqueueAsync(IOrder order);
}

public class QueueOrderDecorator :IQueueOrderDecorator
{
    private readonly IQueueOrderDecorator _queueOrderDecorator;
    private readonly IBaseApplicationService _applicationService;

    public QueueOrderDecorator(IQueueOrderDecorator queueOrderDecorator, IBaseApplicationService applicationService)
    {
        _queueOrderDecorator = queueOrderDecorator;
        _applicationService = applicationService;
    }


    public Task EnqueueAsync(IOrder order)
    {
        throw new NotImplementedException();
    }
}