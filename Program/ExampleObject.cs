using ILN.API;

public class ExampleObject
{
    public int     Age   { get; init; }
    public string  Name  { get; init; }
    public string? State { get; init; }

    [LoggerIgnore]
    public string? SSN { get; init; }
}