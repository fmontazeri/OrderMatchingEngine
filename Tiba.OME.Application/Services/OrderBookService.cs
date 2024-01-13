using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;
using Tiba.OME.Application.Exceptions;
using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Services;

public class OrderBookService(IOrderBookRepository orderBookRepository) : IOrderBookService
{
    public async Task<IOrder> AddOrder(AddOrderCommand command)
    {
        var engine = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (engine == null) throw new OrderBookNotFound();

        var order = engine.AddOrder(command);
        await orderBookRepository.UpdateAsync(engine);
        await orderBookRepository.SaveAsync();
        return order;
    }

    public async Task<IOrder> UpdateOrder(UpdateOrderCommand command)
    {
        var engine = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (engine == null) throw new OrderBookNotFound();

        var order = engine.ModifyOrder(command.OrderId, command.Quantity, command.Price);
        await orderBookRepository.UpdateAsync(engine);
        await orderBookRepository.SaveAsync();
        return order;
    }

    public async Task<IOrder> CancelOrder(CancelOrderCommand command)
    {
        var engine = await orderBookRepository.GetByInstrumentCode(command.InstrumentCode);
        if (engine == null) throw new OrderBookNotFound();

        var order = engine.CancelOrder(command.OrderId);
        await orderBookRepository.UpdateAsync(engine);
        await orderBookRepository.SaveAsync();
        return order;
    }
}