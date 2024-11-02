using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

namespace PrismSharp.SourceGenerator;

public interface IObject;

/// <summary>
/// Any string not in the root object is an index to an object in the root object
/// </summary>
public interface IRef : IObject
{
    int Index { get; }
}

/// <summary>
/// Any string in the root object is a real string
/// </summary>
public interface IString : IObject
{
    string Value { get; }
}

/// <summary>
///
/// </summary>
public interface IBoolean : IObject
{
    bool Value { get; }
}

/// <summary>
///
/// </summary>
public interface INumber : IObject
{
    double Value { get; }
}

/// <summary>
///
/// </summary>
public interface IStructure : IObject
{
    ICollection<string> Properties { get; }
    IObject this[string key] { get; set; }
    bool TryGetValue<TObject>(string key, out TObject? value) where TObject : IObject;
}

// Flatted helps us avoid circular references in JSON
public static class Flatted
{
    private static IStructure Loop(IStructure scope, IList<IObject> input, HashSet<IObject> visited)
    {
        foreach (var key in scope.Properties)
        {
            if (scope[key] is IRef @ref)
                Ref(key, input[@ref.Index], input, visited, scope);
        }
        return scope;
    }

    private static void Ref(string key, IObject value, IList<IObject> input, HashSet<IObject> visited, IStructure output)
    {
        if (value is IStructure structure && visited.Add(structure))
            value = Loop(structure, input, visited);
        output[key] = value;
    }

    private static IObject Wrap(JSONNode value, bool isRoot)
    {
        switch (value)
        {
            case JSONObject jsonObject:
                {
                    var kv = new List<KeyValuePair<string, IObject>>(jsonObject.Count);
                    var i = 0;
                    foreach (var property in jsonObject)
                    {
                        kv.Add(new(property.Key, Wrap(property.Value, false)));
                        i++;
                    }
                    return new StructureWrapper(kv);
                }
            case JSONArray jsonArray:
                {
                    var kv = new List<KeyValuePair<string, IObject>>(jsonArray.Count);
                    var i = 0;
                    foreach (var property in jsonArray)
                    {
                        kv.Add(new(i.ToString(), Wrap(property.Value, false)));
                        i++;
                    }
                    return new StructureWrapper(kv);
                }
            case JSONString jsonString when isRoot:
                return new StringWrapper(jsonString.Value);
            case JSONString jsonString:
                return new RefWrapper(int.Parse(jsonString.Value));
            case JSONNumber jsonNumber:
                return new NumberWrapper(jsonNumber.AsDouble);
            case JSONBool jsonBool:
                return new BooleanWrapper(jsonBool.AsBool);
            default:
                return new StructureWrapper(new List<KeyValuePair<string, IObject>>());
        }
    }

    public static IStructure? Parse(string json)
    {
        var parsed = JSON.Parse(json);

        var i = 0;
        var wrapped = new IObject[parsed.Count];
        foreach (var entry in parsed.AsArray)
            wrapped[i++] = Wrap(entry.Value, true);

        if (wrapped[0] is not IStructure root)
            return null;

        var visited = new HashSet<IObject> { root };
        return Loop(root, wrapped, visited);
    }
}


file record RefWrapper(int Index) : IRef
{
    public int Index { get; } = Index;
}

file record BooleanWrapper(bool Value) : IBoolean
{
    public bool Value { get; } = Value;
}

file record NumberWrapper(double Value) : INumber
{
    public double Value { get; } = Value;
}

file record StringWrapper(string Value) : IString
{
    public string Value { get; } = Value;
}

file record StructureWrapper(List<KeyValuePair<string, IObject>> Value) : IStructure
{
    public ICollection<string> Properties => Value.Select(x => x.Key).ToList();
    public List<KeyValuePair<string, IObject>> Value { get; } = Value;
    public IObject this[string key]
    {
        get => Value.Find(x => x.Key == key).Value;
        set
        {
            Value.RemoveAll(x => x.Key == key);
            Value.Add(new(key, value));
        }
    }

    public bool TryGetValue<TObject>(string key, out TObject? value) where TObject : IObject
    {
        value = default;

        if (Value.FirstOrDefault(x => x.Key == key) is var value2 && value2.Key != key)
            return false;
        if (value2.Value is not TObject value3)
            return false;

        value = value3;
        return true;
    }

    public virtual bool Equals(StructureWrapper? other) => ReferenceEquals(this, other);

    public override int GetHashCode() => Value.GetHashCode();
}
