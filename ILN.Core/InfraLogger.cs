using System.Runtime.CompilerServices;
using ILN.API;

namespace ILN.Core;

public class InfraLogger : IInfraLogger
{
    public readonly string  ApplicationID;
    public readonly string? ProjectRoot;

    public InfraLogger
    (
        string                applicationID,
        List<IMessageHandler> messageHandlers,
        string?               projectRoot = null
    )
    {
        ApplicationID    = applicationID;
        _messageHandlers = messageHandlers;
        ProjectRoot      = projectRoot;
    }

    private readonly List<IMessageHandler> _messageHandlers;

    //

    public IMessage Debug
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    )
    {
        Message message = BuildMessage(Level.Debug, text, null, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        return message;
    }

    public IMessage Statistic
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    )
    {
        Message message = BuildMessage(Level.Statistic, text, null, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        return message;
    }

    public IMessage Info
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    )
    {
        Message message = BuildMessage(Level.Info, text, null, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        return message;
    }

    public IMessage Warning
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    )
    {
        Message message = BuildMessage(Level.Warning, text, e, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        return message;
    }

    public IMessage Error
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    )
    {
        e ??= new Exception(text);

        Message message = BuildMessage(Level.Error, text, e, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        return message;
    }

    public IMessage Fatal
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    )
    {
        e ??= new Exception(text);

        Message message = BuildMessage(Level.Fatal, text, e, meta, sourceName, sourceFile, sourceLine);
        Handle(message);

        throw e;
    }

    //

    private Message BuildMessage
    (
        Level      level,
        string     text,
        Exception? e          = null,
        Fields?    meta       = null,
        string     sourceName = "",
        string     sourceFile = "",
        int        sourceLine = 0
    )
    {
        if (ProjectRoot != null && sourceFile.Contains(ProjectRoot))
        {
            int index = sourceFile.IndexOf(ProjectRoot, StringComparison.Ordinal);
            sourceFile = sourceFile.Substring(index, sourceFile.Length - index);
        }

        var message = new Message
        (
            ApplicationID,
            level: level,
            text: text,
            time: DateTime.UtcNow,
            fields: meta,
            exception: e == null ? null : new ExceptionSummary(e),
            memberName: string.IsNullOrWhiteSpace(sourceName) ? null : sourceName.Trim(),
            sourceFilePath: string.IsNullOrWhiteSpace(sourceFile) ? null : sourceFile.Trim(),
            sourceFileLine: sourceLine == 0 ? null : (uint?) sourceLine
        );

        return message;
    }

    private void Handle(IMessage message)
    {
        foreach (IMessageHandler handler in _messageHandlers)
        {
            handler.Handle(message);
        }
    }
}