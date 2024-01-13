using Tiba.OME.Application.Contracts;
using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;

namespace Tiba.OME.Application.CommandHandlers;

public class OrderBookCommandHandler(IOrderBookService orderBookService) :
    ICommandHandler<AddOrderCommand>
    , ICommandHandler<UpdateOrderCommand>,
    ICommandHandler<CancelOrderCommand>
{
    public async Task Handle(AddOrderCommand command)
    {
        await orderBookService.AddOrder(command);
    }

    public async Task Handle(UpdateOrderCommand command)
    {
        await orderBookService.UpdateOrder(command);
    }

    public async Task Handle(CancelOrderCommand command)
    {
        await orderBookService.CancelOrder(command);
    }
}