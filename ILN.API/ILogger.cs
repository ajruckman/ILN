using System.Runtime.CompilerServices;

namespace ILN.API;

public interface ILogger
{
    IMessage Debug
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    );

    IMessage Statistic
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    );

    IMessage Info
    (
        string                    text,
        Fields?                   meta       = null,
        [CallerMemberName] string sourceName = "",
        [CallerFilePath]   string sourceFile = "",
        [CallerLineNumber] int    sourceLine = 0
    );

    IMessage Warning
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    );

    IMessage Error
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    );

    IMessage Fatal
    (
        string                    text,
        Exception?                e               = null,
        Fields?                   meta            = null,
        bool                      printStacktrace = false,
        [CallerMemberName] string sourceName      = "",
        [CallerFilePath]   string sourceFile      = "",
        [CallerLineNumber] int    sourceLine      = 0
    );

    //

    Task<IMessage> DebugAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );

    Task<IMessage> StatisticAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );

    Task<IMessage> InfoAsync
    (
        string                    text,
        Fields?                   meta              = null,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );

    Task<IMessage> WarningAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );

    Task<IMessage> ErrorAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );

    Task<IMessage> FatalAsync
    (
        string                    text,
        Exception?                e                 = null,
        Fields?                   meta              = null,
        bool                      printStacktrace   = false,
        [CallerMemberName] string sourceName        = "",
        [CallerFilePath]   string sourceFile        = "",
        [CallerLineNumber] int    sourceLine        = 0,
        CancellationToken?        cancellationToken = null
    );
}