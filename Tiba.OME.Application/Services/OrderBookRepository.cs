using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Services;

public class OrderBookRepository : IOrderBookRepository
{
    public Task<OrderBook?> GetByInstrumentCode(string instrumentCode)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(List<IOrder> orders)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(OrderBook input)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }
}