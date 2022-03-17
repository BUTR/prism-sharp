namespace PrismSharp.Core;


public class Grammar
{
    // property name is type
    // value is RegExp | GrammarToken | Array<RegExp | GrammarToken>
    public IDictionary<string, GrammarToken[]> GrammarTokenMap { get; }

    public Grammar(IDictionary<string, GrammarToken[]>? map)
    {
        GrammarTokenMap = map ?? new Dictionary<string, GrammarToken[]>(0);
    }

    public GrammarToken[] this[string key]
    {
        get => GrammarTokenMap[key];
        set => GrammarTokenMap[key] = value;
    }

}
