namespace Tiba.OME.Domain.Core;

public interface IEvent
{
    Guid EventId { get;}
    public DateTime PublishDateTime { get; }
}