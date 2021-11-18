using ILN.API;

namespace ILN.GRPC.Service;

public class ProcessorService
{
    private static List<IMessageProcessor> _processors = null!;

    public async Task InvokeAllProcessors(IMessage message, CancellationToken cancellationToken)
    {
        foreach (IMessageProcessor processor in _processors)
        {
            await processor.Process(message, cancellationToken);
        }
    }

    public static void SetPlugins(List<IMessageProcessor> plugins)
    {
        _processors = plugins;

        // #if DEBUG
        // _processors.Add(new ConsolePrinter());
        // #endif
    }
}