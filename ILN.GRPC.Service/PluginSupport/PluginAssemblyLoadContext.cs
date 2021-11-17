using System.Reflection;
using System.Runtime.Loader;

namespace ILN.GRPC.Service.PluginSupport
{
    // From: https://cjansson.se/blog/post/creating-isolated-plugins-dotnetcore
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly List<Assembly> _loadedAssemblies;
        private readonly string         _path;

        internal readonly Dictionary<string, Assembly> SharedAssemblies;

        public PluginAssemblyLoadContext(string path, Type[] sharedTypes)
        {
            _path = path;

            _loadedAssemblies = new List<Assembly>();

            SharedAssemblies = new Dictionary<string, Assembly>();
            foreach (Type sharedType in sharedTypes)
                SharedAssemblies[Path.GetFileName(sharedType.Assembly.Location)] = sharedType.Assembly;
        }

        public void Initialize()
        {
            foreach (string dll in Directory.EnumerateFiles(_path, "*.dll"))
            {
                if (SharedAssemblies.ContainsKey(Path.GetFileName(dll)))
                    continue;

                _loadedAssemblies.Add(LoadFromAssemblyPath(Path.GetFullPath(dll)));
            }
        }

        public IEnumerable<T> Implementations<T>()
        {
            List<T> result = new();

            foreach (Assembly loadedAssembly in _loadedAssemblies)
            {
                Type[]? types;

                try
                {
                    types = loadedAssembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t)).ToArray();
                }
                catch (Exception e) when (e.Message.StartsWith("Unable to load one or more of the requested types."))
                {
                    continue;
                }

                foreach (Type type in types)
                {
                    try
                    {
                        if (type.FullName == null) continue;

                        object? inst = Activator.CreateInstanceFrom(type.Module.FullyQualifiedName, type.FullName)
                          ?.Unwrap();

                        if (inst == null) continue;

                        var conv = (T) inst;
                        result.Add(conv);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            return result;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string filename = $"{assemblyName.Name}.dll";

            return SharedAssemblies.ContainsKey(filename)
                ? SharedAssemblies[filename]
                : Assembly.Load(assemblyName);
        }
    }
}