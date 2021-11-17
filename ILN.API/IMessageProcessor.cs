namespace ILN.API;

public interface IMessageProcessor
{
    public string ID { get; }

    public Task Process(IMessage message, CancellationToken cancellationToken);
}