namespace Tiba.OME.Domain.OrderBookAgg;

public interface IOrderBookRepository
{
    Task<OrderBook?> GetByInstrumentCode(string instrumentCode);
    Task AddAsync(List<IOrder> orders);
    Task UpdateAsync(OrderBook input);
    Task SaveAsync();
}