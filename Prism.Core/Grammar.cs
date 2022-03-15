namespace Prism.Core;


public class Grammar
{
    // property name is type
    // value is RegExp | GrammarToken | Array<RegExp | GrammarToken>
    public IReadOnlyDictionary<string, GrammarToken[]> GrammarTokenMap { get; }

    public Grammar(IReadOnlyDictionary<string, GrammarToken[]>? map)
    {
        GrammarTokenMap = map ?? new Dictionary<string, GrammarToken[]>(0);
    }

}