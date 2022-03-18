namespace PrismSharp.Core;


public class Grammar
{
    public IDictionary<string, GrammarToken[]> GrammarTokenMap { get; }

    public Grammar()
    {
        GrammarTokenMap = new Dictionary<string, GrammarToken[]>(8);
    }

    public GrammarToken[] this[string key]
    {
        get => GrammarTokenMap[key];
        set => GrammarTokenMap[key] = value;
    }
}
