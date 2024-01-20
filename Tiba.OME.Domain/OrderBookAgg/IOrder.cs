namespace Tiba.OME.Domain.OrderBookAgg;

public interface IOrder : IOrderOptions
{
    public Guid Id { get; }
    public DateTime CreatedOn { get; }
    public DateTime ModifiedOn { get; }
    public OrderState OrderState { get; }

    public void SetLeftOver(int matchedQuantity);
    public void SetAsFulfilled();
    public void SetAsCancelled();
    public bool IsValidToMatch();
    public bool IsMatchedTo(IOrder order);
    public bool IsOrderFulfilled();
}