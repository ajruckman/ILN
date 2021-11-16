using System.Diagnostics;
using ILN.API;

namespace ILN.Core;

public class ConsoleMessageHandler : IMessageHandler
{
    public readonly bool PrintSourceInfo;
    public readonly bool PrintStacktrace;

    public ConsoleMessageHandler(bool printSourceInfo = false, bool printStacktrace = false)
    {
        PrintSourceInfo = printSourceInfo;
        PrintStacktrace = printStacktrace;
    }

    public Task Handle(IMessage message)
    {
        var formatted =
            $"{FormatLevel(message.Level)} [{message.Time:HH:mm:ss.fff}] {Format(message)}";

        Console.WriteLine(formatted);
        Debug.WriteLine(formatted);

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