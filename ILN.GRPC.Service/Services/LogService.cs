using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;

namespace ILN.GRPC.Service.Services;

internal class LogService : _LogService._LogServiceBase
{
    private readonly ILogger<LogService> _logger;

    internal LogService(ILogger<LogService> logger)
    {
        _logger = logger;
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
        Console.WriteLine(JsonConvert.SerializeObject(request));
        
        return new Empty();
    }
}