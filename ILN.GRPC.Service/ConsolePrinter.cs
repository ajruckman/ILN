using ILN.API;
using Newtonsoft.Json;

namespace ILN.GRPC.Service;

// TODO: Disable this. We don't want to print all messages to the console in production.
// Also, it's not in Plugins namespace.

public class ConsolePrinter : IMessageProcessor
{
    public string ID => "ILN.GRPC.Service.ConsolePrinter";

    public Task Process(IMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine(JsonConvert.SerializeObject(message, Formatting.Indented));

        return Task.CompletedTask;
    }
}