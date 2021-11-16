namespace ILN.API;

public class ExceptionSummary
{
    public ExceptionSummary(Exception exception)
    {
        Message    = exception.Message;
        StackTrace = string.IsNullOrWhiteSpace(exception.StackTrace) ? null : exception.StackTrace;
    }

    public string  Message    { get; }
    public string? StackTrace { get; }
}