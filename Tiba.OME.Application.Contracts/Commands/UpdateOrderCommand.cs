namespace Tiba.OME.Application.Contracts.Commands;

public record class UpdateOrderCommand(Guid OrderId, int Quantity, decimal Price , string InstrumentCode);
