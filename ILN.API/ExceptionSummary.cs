namespace ILN.API;

public class ExceptionSummary
{
    public ExceptionSummary(Exception exception)
    {
        Message    = exception.Message;
        StackTrace = string.IsNullOrWhiteSpace(exception.StackTrace) ? null : exception.StackTrace;
    }

    public ExceptionSummary(string message, string? stackTrace)
    {
        Message    = message;
        StackTrace = stackTrace;
    }

    public string  Message    { get; }
    public string? StackTrace { get; }
}