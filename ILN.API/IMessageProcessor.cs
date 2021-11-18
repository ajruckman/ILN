namespace ILN.API;

public interface IMessageProcessor
{
    public string ID { get; }

    public void Initialize(IReadOnlyDictionary<string, object> parameters);

    public Task Process(IMessage message, CancellationToken cancellationToken);
}