using ILN.API;

namespace ILN.Core;

public class MessageEventAnnouncer : IMessageActor
{
    public void Handle(IMessage message)
    {
        throw new NotImplementedException();
    }

    public event Action<IMessage>? OnAny;
    public event Action<IMessage>? OnDebug;
    public event Action<IMessage>? OnStatistic;
    public event Action<IMessage>? OnInfo;
    public event Action<IMessage>? OnWarning;
    public event Action<IMessage>? OnError;
    public event Action<IMessage>? OnFatal;

    public Task HandleAsync(IMessage message, CancellationToken? cancellationToken)
    {
        OnAny?.Invoke(message);

        switch (message.Level)
        {
            case Level.Debug:
                OnDebug?.Invoke(message);
                break;
            case Level.Statistic:
                OnStatistic?.Invoke(message);
                break;
            case Level.Info:
                OnInfo?.Invoke(message);
                break;
            case Level.Warning:
                OnWarning?.Invoke(message);
                break;
            case Level.Error:
                OnError?.Invoke(message);
                break;
            case Level.Fatal:
                OnFatal?.Invoke(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(message));
        }

        return Task.CompletedTask;
    }
}