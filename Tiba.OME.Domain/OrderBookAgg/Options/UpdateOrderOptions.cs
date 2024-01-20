using Tiba.OME.Domain.Core;

namespace Tiba.OME.Domain.OrderBookAgg.Options;

public class UpdateOrderOptions(Guid orderId, int quantity, decimal price) : IDomainCommand
{
    public Guid OrderId { get; set; } = orderId;
    public int Quantity { get; set; } = quantity;
    public decimal Price { get; set; } = price;
}