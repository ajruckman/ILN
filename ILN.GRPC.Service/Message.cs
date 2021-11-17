using ILN.API;

namespace ILN.GRPC.Service;

public class Message : IMessage
{
    internal Message
    (
        string  applicationID, DateTime          time,           Level   level,
        string  text,          ExceptionSummary? exception,      Fields? fields,
        string? memberName,    string?           sourceFilePath, uint?   sourceFileLine
    )
    {
        ApplicationID  = applicationID;
        Time           = time;
        Level          = level;
        Text           = text;
        Exception      = exception;
        Fields         = fields;
        MemberName     = memberName;
        SourceFilePath = sourceFilePath;
        SourceFileLine = sourceFileLine;
    }

    public string            ApplicationID { get; }
    public DateTime          Time          { get; }
    public Level             Level         { get; }
    public string            Text          { get; }
    public ExceptionSummary? Exception     { get; }
    public Fields?           Fields        { get; }

    public string? MemberName     { get; }
    public string? SourceFilePath { get; }
    public uint?   SourceFileLine { get; }
}