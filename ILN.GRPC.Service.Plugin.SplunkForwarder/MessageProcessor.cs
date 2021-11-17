using ILN.API;

namespace ILN.GRPC.Service.Plugin.SplunkForwarder;

public class MessageProcessor : IMessageProcessor
{
    public MessageProcessor() { }

    public string ID => "ILN.GRPC.Service.Plugin.SplunkForwarder";

    public async Task Process(IMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"FORWARD {message.Text}");
    }
}