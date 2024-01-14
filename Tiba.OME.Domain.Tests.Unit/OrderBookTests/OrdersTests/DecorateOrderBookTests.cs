// using NSubstitute;
// using Tiba.OME.Domain.OrderBookAgg;
// using Tiba.OME.Domain.Tests.Unit.Consts;
// using Tiba.OME.Domain.Tests.Unit.OrderBookTests.Fixtures;
//
// namespace Tiba.OME.Domain.Tests.Unit.OrderBookTests.OrdersTests;
//
// public class DecorateOrderBookTests : BaseOrderBookTest
// {
//     [Fact]
//     public async Task AddOrder_Should_Queue_Incoming_Order_Successfully()
//     {
//         var queue = Substitute.For<IOrderQueue>();
//         var orderBook =
//             new OrderBookDecorator(Guid.NewGuid(), InstrumentConsts.OfoghBKoorosh, new List<IOrder>(), queue);
//         IncomingOrder = _testOrderBuilder
//             .WithOrderSide(OrderSide.Sell)
//             .WithInstrumentCode(InstrumentConsts.OfoghBKoorosh)
//             .BuildOption();
//
//         var expected = orderBook.AddOrder(IncomingOrder);
//
//         queue.Received(1).EnqueueOrder(IncomingOrder);
//     }
// }