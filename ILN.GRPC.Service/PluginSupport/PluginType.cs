namespace ILN.GRPC.Service.PluginSupport;

public class PluginType<T> where T : class
{
    private readonly Type _type;

    public PluginType(Type type)
    {
        _type = type;
    }

    public T? Initialize()
    {
        try
        {

            if (_type.FullName == null) return null;

            object? inst = Activator.CreateInstanceFrom(_type.Module.FullyQualifiedName, _type.FullName)
              ?.Unwrap();

            return inst as T;
        }
        catch
        {
            return null;
        }
    }
}