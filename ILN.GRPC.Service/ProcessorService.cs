using ILN.API;

namespace ILN.GRPC.Service;

public class ProcessorService
{
    private static IReadOnlyList<IMessageProcessor> _processors;

    public async Task InvokeAllProcessors(IMessage message, CancellationToken cancellationToken)
    {
        foreach (IMessageProcessor processor in _processors)
        {
            await processor.Process(message, cancellationToken);
        }
    }

    public static void SetPlugins(IEnumerable<IMessageProcessor> plugins)
    {
        _processors = plugins.ToArray();

        #if DEBUG
        _processors = _processors.Append(new ConsolePrinter()).ToArray();
        #endif
    }
}