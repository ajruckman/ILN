using System.Diagnostics;
using ILN.API;

namespace ILN.Core;

public partial class MessageConsolePrinter : IMessageActor
{
    public readonly bool PrintSourceInfo;
    public readonly bool PrintStacktrace;

    public MessageConsolePrinter(bool printSourceInfo = true, bool printStacktrace = true)
    {
        PrintSourceInfo = printSourceInfo;
        PrintStacktrace = printStacktrace;
    }

    public void Handle(IMessage message)
    {
        var formatted =
            $"{FormatLevel(message.Level)} [{message.Time:HH:mm:ss.fff}] {Format(message)}";

        Console.WriteLine(formatted);
        Debug.WriteLine(formatted);
    }

    public Task HandleAsync(IMessage message, CancellationToken? _)
    {
        Handle(message);
        return Task.CompletedTask;
    }

    public string Format(IMessage m)
    {
        var result = "";

        if (PrintSourceInfo)
            result += $"[{m.SourceFilePath}:{m.SourceFileLine} > {m.MemberName}] ";

        result += m.Text;

        if (m.Exception != null)
            result += $" <Exception: {m.Exception.Message}>";

        if (m.Fields?.Count > 0) result += " " + m.Fields;

        if (m.Exception?.StackTrace != null && PrintStacktrace)
        {
            result += "\n" + "Stacktrace:";

            using StringReader reader = new(m.Exception.StackTrace);

            for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
                result += "\n" + line;
        }

        return result;
    }

    private static string FormatLevel(Level level)
    {
        return $"[{LevelAbbreviation(level),-4}]";
    }

    private static string LevelAbbreviation(Level level) => level switch
    {
        Level.Debug     => "DEBG",
        Level.Statistic => "STAT",
        Level.Info      => "INFO",
        Level.Warning   => "WARN",
        Level.Error     => "ERRR",
        Level.Fatal     => "FATL",
        _               => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };
}

public partial class MessageConsolePrinter
{
    public static Logger New
    (
        string applicationID,          string? projectRoot     = null,
        bool   printSourceInfo = true, bool    printStacktrace = true
    )
    {
        return new Logger(applicationID, new List<IMessageActor>
        {
            new MessageConsolePrinter(printSourceInfo, printStacktrace),
        }, projectRoot);
    }
}