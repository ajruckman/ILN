using System.Collections;
using System.Net.Security;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using ILN.API;
using ILN.GRPC.Service;

namespace ILN.Core;

public class ReceiverServiceMessageHandler : IMessageHandler
{
    private readonly GrpcChannel                   _grpcChannel;
    private readonly _LogService._LogServiceClient _grpcClient;

    public ReceiverServiceMessageHandler(string address, string applicationID, bool ignoreSSL = false)
    {
        GrpcChannelOptions options = new();
        SocketsHttpHandler handler = new()
        {
            PooledConnectionIdleTimeout    = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay             = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout           = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true,
        };

        if (ignoreSSL)
        {
            handler.SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
            };
        }

        options.HttpHandler = handler;

        //

        _grpcChannel = GrpcChannel.ForAddress(address, options);
        _grpcClient  = new _LogService._LogServiceClient(_grpcChannel);

        bool acknowledged;

        try
        {
            _LogService_HelloResponse response = _grpcClient.SayHello(new _HelloPayload
            {
                ApplicationId    = applicationID,
                MachineName      = Environment.MachineName,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                ClientVersion = typeof(ReceiverServiceMessageHandler).Assembly.GetName().Version?.ToString() ??
                                "UNKNOWN",
            });

            acknowledged = response.Acknowledged;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to connect to gRPC log service.", e);
        }

        if (!acknowledged)
        {
            throw new Exception("gRPC log service did not acknowledge initial request.");
        }
    }

    public async Task Handle(IMessage message)
    {
        var payload = new _MessagePayload
        {
            ApplicationId = message.ApplicationID,
            Time          = ((DateTimeOffset) message.Time).ToUnixTimeSeconds(),
            Level = message.Level switch
            {
                Level.Debug     => _Level.Debug,
                Level.Statistic => _Level.Statistic,
                Level.Info      => _Level.Info,
                Level.Warning   => _Level.Warning,
                Level.Error     => _Level.Error,
                Level.Fatal     => _Level.Fatal,
                _               => _Level.Undefined,
            },
            Text = message.Text,
            Exception = message.Exception == null
                ? null
                : new _ExceptionSummary
                {
                    Message    = message.Exception.Message,
                    Stacktrace = message.Exception.StackTrace,
                },
            MemberName     = message.MemberName,
            SourceFilePath = message.SourceFilePath,
            SourceFileLine = message.SourceFileLine,
        };

        if (message.Fields != null)
        {
            foreach (DictionaryEntry entry in message.Fields)
            {
                payload.Fields.Add(new _Field
                {
                    Key   = entry.Key.ToString(),
                    Value = entry.Value?.ToString(),
                });
            }
        }

        //

        Empty _ = await _grpcClient.LogAsync(payload);
    }
}