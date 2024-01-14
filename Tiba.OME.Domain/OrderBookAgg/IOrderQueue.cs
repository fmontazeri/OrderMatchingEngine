// using System.Collections.Concurrent;
//
// namespace Tiba.OME.Domain.OrderBookAgg;
//
// public interface IOrderQueue
// {
//     public BlockingCollection<object> OrderJobs { get; }
//     void EnqueueOrder(object orderOptions);
// }
//
// public class OrderQueue : IOrderQueue
// {
//     public BlockingCollection<object> OrderJobs { get; private set; }
//
//     public void EnqueueOrder(object orderOptions)
//     {
//         OrderJobs.Add(orderOptions);
//     }
// }
//
// public class OrderBookCollectionQueue : OrderBook, IOrderQueue
// {
//     public BlockingCollection<object> OrderJobs { get; set; } = new();
//
//     public void EnqueueOrder(object orderOptions)
//     {
//         OrderJobs.Add(orderOptions);
//     }
//
//     public OrderBookCollectionQueue()
//     {
//         var thread = new Thread(OnStart)
//         {
//             IsBackground = true
//         };
//         thread.Start();
//     }
//
//     public void Stop()
//     {
//         //This will cause '_jobs.GetConsumingEnumerable' to stop blocking and exit when it's empty
//         OrderJobs.CompleteAdding();
//     }
//
//     private void OnStart()
//     {
//         foreach (var order in OrderJobs.GetConsumingEnumerable(CancellationToken.None))
//         {
//             Console.WriteLine("--------");
//             //TODO : TODO : use command pattern
//             // Console.WriteLine(order.OrderSide + " " + order.Quantity + " " + order.Price);
//             //Send order option to command handler
//             //order execute AddOrder in OrderBookService
//         }
//     }
// }
//
// public class OrderBookDecorator : OrderBook
// {
//     private readonly IOrderQueue _jobQueue;
//
//     public OrderBookDecorator(Guid id, string instrumentCode, List<IOrder> incomingOrders, IOrderQueue jobQueue) : base(
//         id, instrumentCode, incomingOrders)
//     {
//         _jobQueue = jobQueue;
//     }
//
//
//     public override IOrder AddOrder(IOrderOptions options)
//     {
//         //var result = base.AddOrder(options); // Call the original method to add the order to the OrderBook
//         _jobQueue.EnqueueOrder(options); // Enqueue the order in the job queue
//         return (IOrder)options;
//     }
// }