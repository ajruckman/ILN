using ILN.API;
using ILN.Core;

namespace ILN.GRPC.Service.PluginSupport
{
    public static class PluginLoader
    {
        private static readonly Logger Logger = MessageConsolePrinter.New("ILN.GRPC.Service", "ILN");

        public static IEnumerable<T> ReadPlugins<T>(string path, Type[] sharedTypes) where T : class
        {
            foreach (var pluginDir in Directory.GetDirectories(path))
            {
                var context = new PluginAssemblyLoadContext(pluginDir, sharedTypes);

                T[] plugins;

                try
                {
                    context.Initialize();
                    plugins = context.Implementations<T>().ToArray();

                    if (plugins.Length > 0)
                    {
                        Logger.Debug("Loaded plugins from assembly.", new Fields
                        {
                            {"Path", Path.GetFileName(pluginDir)},
                            {"Type", typeof(T).Name},
                            {"Count", plugins.Length},
                            {"Assemblies", string.Join('|', plugins.Select(v => v.GetType().Assembly))},
                        });
                    }
                    else
                    {
                        Logger.Debug("No plugins found in assembly.", new Fields
                        {
                            {"Path", Path.GetFileName(pluginDir)},
                            {"Type", typeof(T).Name},
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning("Failed to load plugins for assembly.", e, new Fields
                    {
                        {"Path", Path.GetFileName(pluginDir)},
                        {"Type", typeof(T).Name},
                    });
                    continue;
                }

                foreach (var plugin in plugins)
                {
                    yield return plugin;
                }
            }
        }
    }
}