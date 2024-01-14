using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;
using Tiba.OME.Application.Exceptions;
using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Services;

public class OrderBookService(IOrderBookRepository orderBookRepository)
    : IOrderBookService
{
    public async Task<IOrder> AddOrder(AddOrderCommand command)
    {
        var orderBook = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (orderBook == null) throw new OrderBookNotFound();

        var order = orderBook.AddOrder(command);
        await orderBookRepository.UpdateAsync(orderBook);
        await orderBookRepository.SaveAsync();
        return order;
    }

    public async Task<IOrder> UpdateOrder(UpdateOrderCommand command)
    {
        var orderBook = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (orderBook == null) throw new OrderBookNotFound();

        var order = orderBook.ModifyOrder(command.OrderId, command.Quantity, command.Price);
        await orderBookRepository.UpdateAsync(orderBook);
        await orderBookRepository.SaveAsync();
        return order;
    }

    public async Task<IOrder> CancelOrder(CancelOrderCommand command)
    {
        var orderBook = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (orderBook == null) throw new OrderBookNotFound();

        var order = orderBook.CancelOrder(command.OrderId);
        await orderBookRepository.UpdateAsync(orderBook);
        await orderBookRepository.SaveAsync();
        return order;
    }
}