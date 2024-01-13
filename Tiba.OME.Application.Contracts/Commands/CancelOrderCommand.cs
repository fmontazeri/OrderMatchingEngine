namespace Tiba.OME.Application.Contracts.Commands;

public record class CancelOrderCommand(Guid OrderId, string InstrumentCode) : ICommand;
