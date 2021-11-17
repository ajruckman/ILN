using ILN.API;
using ILN.GRPC.Service;
using ILN.GRPC.Service.PluginSupport;
using ILN.GRPC.Service.Services;

//

IEnumerable<IMessageProcessor> plugins = PluginLoader.ReadPlugins<IMessageProcessor>("./Plugins", new[]
{
    typeof(IMessageProcessor),
});

ProcessorService.SetPlugins(plugins);

//

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<ProcessorService>();

WebApplication app = builder.Build();

app.MapGrpcService<LogService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();