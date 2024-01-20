using Tiba.OME.Domain.OrderBookAgg;

namespace Tiba.OME.Application.Contracts.Commands;

public record class AddOrderCommand(
    OrderSide OrderSide,
    int Quantity,
    decimal Price,
    string InstrumentCode,
    string CustomerCode) : IOrderOptions, ICommand;



