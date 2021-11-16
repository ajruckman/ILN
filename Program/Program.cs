// See https://aka.ms/new-console-template for more information

using ILN.API;
using ILN.Core;

ExampleObject o = new()
{
    Age   = 25,
    Name  = "Ronny Canales",
    State = "AZ",
    SSN   = "123 456 7890",
};

Console.WriteLine(new Fields(o).ToString());

//

List<IMessageHandler> handlers = new()
{
    new ConsoleMessageHandler(false, false),
    new ReceiverServiceMessageHandler("http://localhost:5257", "ILN.Program"),
};
InfraLogger lc = new("ILN.Program", handlers);

lc.Info("test");

lc.Error("test", new Exception("test"));

lc.Info("test", new Fields
{
    {"Test", 1},
});

lc.Error("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});

//

handlers = new()
{
    new ConsoleMessageHandler(true),
};
lc = new InfraLogger("ILN.Program", handlers);

lc.Info("test");

lc.Error("test", new Exception("test"));

lc.Info("test", new Fields
{
    {"Test", 1},
});

lc.Error("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});

//

handlers = new()
{
    new ConsoleMessageHandler(true, true),
};
lc = new InfraLogger("ILN.Program", handlers, "ILN");

lc.Info("test");

lc.Error("test", new Exception("test"));

lc.Info("test", new Fields
{
    {"Test", 1},
});

lc.Error("test", new Exception("test"), new Fields
{
    {"Test", 1},
    {"Object", new Fields(o)},
});