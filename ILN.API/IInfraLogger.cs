using System.Runtime.CompilerServices;

namespace ILN.API;

public interface IInfraLogger
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
}