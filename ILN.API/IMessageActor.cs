namespace ILN.API;

public interface IMessageActor
{
    public void Handle(IMessage message);

    public Task HandleAsync(IMessage message, CancellationToken? cancellationToken);
}