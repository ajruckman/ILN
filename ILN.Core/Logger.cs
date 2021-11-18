using System.Runtime.CompilerServices;
using ILN.API;

namespace ILN.Core;

public class Logger : ILogger
{
    public readonly string  Host;
    public readonly string  ApplicationID;
    public readonly string? ProjectRoot;

    public Logger
    (
        string              applicationID,
        List<IMessageActor> messageActors,
        string?             host        = null,
        string?             projectRoot = null
    )
    {
        Host           = host ?? Environment.MachineName;
        ApplicationID  = applicationID;
        _messageActors = messageActors;
        ProjectRoot    = projectRoot;
    }

    private readonly List<IMessageActor> _messageActors;

    //

    private IMessage Log
    (
        Level      level,
        string     text,
        Exception? e,
        Fields?    meta,
        string     sourceName,
        string     sourceFile,
        int        sourceLine
    )
    {
        Message message = BuildMessage
        (
            level,      text,       e, meta,
            sourceName, sourceFile, sourceLine
        );

        foreach (IMessageActor actor in _messageActors)
        {
            actor.Handle(message);
        }

        return message;
    }

    public IMessage Debug
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    ) => Log(Level.Debug, text, null, meta, sourceName, sourceFile, sourceLine);

    public IMessage Statistic
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    ) => Log(Level.Statistic, text, null, meta, sourceName, sourceFile, sourceLine);

    public IMessage Info
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    ) => Log(Level.Info, text, null, meta, sourceName, sourceFile, sourceLine);

    public IMessage Warning
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    ) => Log(Level.Warning, text, e, meta, sourceName, sourceFile, sourceLine);

    public IMessage Error
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    ) => Log(Level.Error, text, e, meta, sourceName, sourceFile, sourceLine);

    public IMessage Fatal
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    ) => Log(Level.Fatal, text, e, meta, sourceName, sourceFile, sourceLine);
    //

    private async Task<IMessage> LogAsync
    (
        Level              level,
        string             text,
        Exception?         e,
        Fields?            meta,
        string             sourceName,
        string             sourceFile,
        int                sourceLine,
        CancellationToken? cancellationToken
    )
    {
        Message message = BuildMessage
        (
            level,      text,       e, meta,
            sourceName, sourceFile, sourceLine
        );

        foreach (IMessageActor actor in _messageActors)
        {
            await actor.HandleAsync(message, cancellationToken);
        }

        return message;
    }

    public Task<IMessage> DebugAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Debug, text, null, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    public Task<IMessage> StatisticAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Statistic, text, null, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    public Task<IMessage> InfoAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Info, text, null, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    public Task<IMessage> WarningAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Warning, text, e, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    public Task<IMessage> ErrorAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Error, text, e, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    public Task<IMessage> FatalAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    ) => LogAsync(Level.Fatal, text, e, meta, sourceName, sourceFile, sourceLine, cancellationToken);

    //

    private Message BuildMessage
    (
        Level      level,
        string     text,
        Exception? e,
        Fields?    meta,
        string     sourceName,
        string     sourceFile,
        int        sourceLine
    )
    {
        if (ProjectRoot != null && sourceFile.Contains(ProjectRoot))
        {
            int index = sourceFile.IndexOf(ProjectRoot, StringComparison.Ordinal);
            sourceFile = sourceFile.Substring(index, sourceFile.Length - index);
        }

        var message = new Message
        (
            Host,
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
}