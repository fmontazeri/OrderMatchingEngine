namespace Tiba.OME.Domain.Core;

public class AggregateRootBase<TKey> : EntityBase<TKey>, IAggregateRoot
{
    private List<DomainEvent> _publishedEvents = new();

    protected AggregateRootBase() { }
    protected AggregateRootBase(TKey id) : base(id)
    {
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : DomainEvent
    {
        this._publishedEvents.Add(@event);
    }

    public List<DomainEvent> GetPublishedEvents()
    {
        return this._publishedEvents;
    }
    public void ClearEvents()
    {
        this._publishedEvents.Clear();
    }
}