using ILN.API;
using ILN.Core;
using ILN.GRPC.MessageForwarder;
using ILN.GRPC.Service;
using ILN.GRPC.Service.PluginSupport;
using ILN.GRPC.Service.Services;
using Newtonsoft.Json;

//

Logger logger = MessageConsolePrinter.New("ILN.GRPC.Service", "ILN");

string configText = File.ReadAllText("config.json");

var config = JsonConvert.DeserializeObject<Config>(configText)!;

PluginType<IMessageProcessor>[] pluginTypes = PluginLoader.ReadPlugins<IMessageProcessor>("./Plugins", new[]
{
    typeof(IMessageProcessor),
}).ToArray();

var plugins = new List<IMessageProcessor>();

foreach (PluginConfig pluginConfig in config.Plugins)
{
    foreach (PluginType<IMessageProcessor> pluginType in pluginTypes)
    {
        IMessageProcessor? plugin = pluginType.Initialize();
        if (plugin == null) continue;

        if (plugin.ID != pluginConfig.ID) continue;

        plugin.Initialize(pluginConfig.Parameters);
        plugins.Add(plugin);

        goto next;
    }

    logger.Fatal("Failed to find match for plugin in config.", null, new Fields
    {
        {"ID", pluginConfig.ID},
    });

    Environment.Exit(1);

    next: ;
}

foreach (PluginType<IMessageProcessor> pluginType in pluginTypes)
{
    foreach (PluginConfig pluginConfig in config.Plugins)
    {
        IMessageProcessor? plugin = pluginType.Initialize();
        if (plugin == null) continue;

        if (pluginConfig.ID == plugin.ID)
        {
            plugin.Initialize(pluginConfig.Parameters);
            plugins.Add(plugin);
        }
    }
}

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

//

Task.Run(() =>
{
    Thread.Sleep(2000);
    List<IMessageActor> handlers = new()
    {
        new MessageConsolePrinter(false, false),
        new MessageForwarder("https://localhost:7141", "ILN.Program", true),
    };
    Logger lc = new("ILN.Program", handlers);

    lc.Info("1950138509175639");
});

//

app.Run();