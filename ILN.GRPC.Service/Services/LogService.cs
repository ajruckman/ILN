using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ILN.API;

namespace ILN.GRPC.Service.Services;

internal class LogService : _LogService._LogServiceBase
{
    private readonly ProcessorService _processorService;
    // private readonly ILogger<LogService> _logger;

    public LogService(ProcessorService processorService)
    {
        _processorService = processorService;
    }

    public async override Task<_LogService_HelloResponse> SayHello(_HelloPayload request, ServerCallContext context)
    {
        return new _LogService_HelloResponse
        {
            Acknowledged = true,
        };
    }

    public override async Task<Empty> Log(_MessagePayload request, ServerCallContext context)
    {
        IMessage message = new Message
        (
            request.Host,
            request.ApplicationId,
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(request.Time),
            request.Level switch
            {
                _Level.Debug     => Level.Debug,
                _Level.Statistic => Level.Statistic,
                _Level.Info      => Level.Info,
                _Level.Warning   => Level.Warning,
                _Level.Error     => Level.Error,
                _Level.Fatal     => Level.Fatal,
                _Level.Undefined => throw new ArgumentOutOfRangeException(nameof(request)),
                _                => throw new ArgumentOutOfRangeException(nameof(request)),
            },
            request.Text,
            request.Exception == null
                ? null
                : new ExceptionSummary(request.Exception.Message, string.IsNullOrEmpty(request.Exception.Stacktrace)
                    ? null
                    : request.Exception.Stacktrace),
            new Fields(),
            request.MemberName,
            request.SourceFilePath,
            request.SourceFileLine
        );

        await _processorService.InvokeAllProcessors(message, context.CancellationToken);

        return new Empty();
    }
}