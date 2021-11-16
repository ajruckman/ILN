namespace ILN.API;

public interface IMessageHandler
{
    public Task Handle(IMessage message);
}