namespace ILN.GRPC.Service;

public class Config
{
    public List<PluginConfig> Plugins { get; set; }
}

public class PluginConfig
{
    public string                     ID         { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}