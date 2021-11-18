using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace ILN.API;

public class Fields : OrderedDictionary
{
    private const BindingFlags BindingFlags =
        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

    public Fields() { }

    public Fields(object? source)
    {
        if (source == null) return;

        Type t = source.GetType();

        foreach (FieldInfo m in t.GetFields(BindingFlags))
            if (m.GetCustomAttribute(typeof(LoggerIgnoreAttribute)) == null)
                Add(m.Name, m.GetValue(source)?.ToString());

        foreach (PropertyInfo m in t.GetProperties(BindingFlags).Where(v => v.GetIndexParameters().Length == 0))
            if (m.GetCustomAttribute(typeof(LoggerIgnoreAttribute)) == null)
                Add(m.Name, m.GetValue(source)?.ToString());
    }

    public Fields(IDictionary<string, object?>? dict)
    {
        if (dict == null) return;

        foreach ((string key, object? value) in dict)
        {
            Add(key, value?.ToString());
        }
    }

    public Fields(IReadOnlyDictionary<string, object?>? dict)
    {
        if (dict == null) return;

        foreach ((string key, object? value) in dict)
        {
            Add(key, value?.ToString());
        }
    }

    public Fields(OrderedDictionary? dict)
    {
        if (dict == null) return;

        foreach (DictionaryEntry o in dict)
        {
            Add(o.Key, o.Value?.ToString());
        }
    }

    public override string ToString()
    {
        return string.Join(' ', Keys.OfType<string>().Select(v =>
            $"[{v}: {(this[v] is Fields ? "[...]" : this[v])}]"
        ));
    }

    //

    public static Fields operator +(Fields l, Fields r)
    {
        var c = new Fields();

        foreach (string key in l.Keys) c[key] = l[key];
        foreach (string key in r.Keys) c[key] = r[key];

        return c;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LoggerIgnoreAttribute : Attribute { }