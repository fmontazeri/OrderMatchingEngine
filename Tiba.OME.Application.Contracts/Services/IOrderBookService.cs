using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Contracts.Services;

public interface IOrderBookService
{
    Task<IOrder> AddOrder(AddOrderCommand command);
    Task<IOrder> UpdateOrder(UpdateOrderCommand command);
    Task<IOrder> CancelOrder(CancelOrderCommand command);
}