namespace ILN.API;

public interface IMessage
{
    string            ApplicationID  { get; }
    DateTime          Time           { get; }
    Level             Level          { get; }
    string            Text           { get; }
    ExceptionSummary? Exception      { get; }
    Fields?           Fields         { get; }
    string?           MemberName     { get; }
    string?           SourceFilePath { get; }
    uint?             SourceFileLine { get; }
}