namespace Tiba.OME.Domain.Core;

public  abstract  class DomainEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime PublishDateTime { get; }
    protected DomainEvent()
    {
        this.EventId = Guid.NewGuid();
        this.PublishDateTime = DateTime.Now;
    }
}