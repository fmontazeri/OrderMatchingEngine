namespace Tiba.OME.Application.Contracts;

public interface ICommandHandler<T> where T : ICommand
{
    Task Handle(T cmd);
}

public interface ICommandHandler<T,TResult> where T : ICommand 
{
    public Task<TResult> Handle(T command);
}