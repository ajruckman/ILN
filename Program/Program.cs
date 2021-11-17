// See https://aka.ms/new-console-template for more information

using ILN.API;
using ILN.Core;
using ILN.GRPC.MessageForwarder;

ExampleObject o = new()
{
    Age   = 25,
    Name  = "Ronny Canales",
    State = "AZ",
    SSN   = "123 456 7890",
};

Console.WriteLine(new Fields(o).ToString());

//

List<IMessageActor> handlers = new()
{
    new MessageConsolePrinter(false, false),
    new MessageForwarder("https://localhost:7141", "ILN.Program", true),
};
Logger lc = new("ILN.Program", handlers);

lc.Info("test");

await lc.ErrorAsync("test", new Exception("test"));

await lc.InfoAsync("test", new Fields
{
    {"Test", 1},
});

await lc.ErrorAsync("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});

//

handlers = new()
{
    new MessageConsolePrinter(true),
    new MessageForwarder("https://localhost:7141", "ILN.Program", true),
};
lc = new Logger("ILN.Program", handlers);

await lc.InfoAsync("test");

await lc.ErrorAsync("test", new Exception("test"));

await lc.InfoAsync("test", new Fields
{
    {"Test", 1},
});

await lc.ErrorAsync("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});

//

handlers = new()
{
    new MessageConsolePrinter(true, true),
    new MessageForwarder("https://localhost:7141", "ILN.Program", true),
};
lc = new Logger("ILN.Program", handlers, "ILN");

await lc.InfoAsync("test");

await lc.ErrorAsync("test", new Exception("test"));

await lc.InfoAsync("test", new Fields
{
    {"Test", 1},
});

await lc.ErrorAsync("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});