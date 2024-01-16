namespace Tiba.OME.Domain.Core;

public interface IDomainCommandHandler<T> where T : IDomainCommand
{
    Task Handle(T command);
}