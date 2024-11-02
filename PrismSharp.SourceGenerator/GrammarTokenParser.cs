using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PrismSharp.SourceGenerator;

public class GrammarJson : IEnumerable<KeyValuePair<string, GrammarTokenJson[]>>
{
    private readonly List<KeyValuePair<string, GrammarTokenJson[]>> _tokens = new();

    public GrammarTokenJson[] this[string key] { get => _tokens.FirstOrDefault(x => x.Key == key).Value; set => _tokens.Add(new(key, value)); }

    public IEnumerator<KeyValuePair<string, GrammarTokenJson[]>> GetEnumerator() => _tokens.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_tokens).GetEnumerator();
}

public class GrammarTokenJson
{
    public string PatternVariableName { get; }
    public string Pattern { get; }
    public bool Lookbehind { get; }
    public bool Greedy { get; }
    public string[] Alias { get; }
    public GrammarJson? Inside { get; }

    public GrammarTokenJson(string patternVariableName, string pattern, bool lookbehind = false, bool greedy = false, string[]? alias = null, GrammarJson? inside = null)
    {
        PatternVariableName = patternVariableName;
        Pattern = pattern;
        Lookbehind = lookbehind;
        Greedy = greedy;
        Alias = alias ?? [];
        Inside = inside;
    }

}

public static class GrammarTokenParser
{
    private static unsafe string NormalizeName(string input)
    {
        var array = stackalloc char[input.Length + 1];
        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsLetterOrDigit(input[i]))
                array[i] = input[i];
            else
                array[i] = '_';
        }
        array[input.Length] = '_';
        return new string(array, 0, input.Length + 1);
    }

    private static GrammarTokenJson ParseGrammarTokens(string nestedName, IStructure grammarToken, Dictionary<IObject, GrammarJson> refGrammars, Dictionary<IObject, GrammarTokenJson[]> refTokens)
    {
        var pattern = grammarToken.TryGetValue("pattern", out IString? patternVar) ? patternVar!.Value : throw new Exception("Pattern is not a string.");
        var lookbehind = grammarToken.TryGetValue("lookbehind", out IBoolean? lookbehindVar) ? lookbehindVar!.Value : false;
        var greedy = grammarToken.TryGetValue("greedy", out IBoolean? greedyVar) ? greedyVar!.Value : false;
        var alias = grammarToken.TryGetValue("alias", out IString? aliasVar) ? new[] { aliasVar!.Value }
            : grammarToken.TryGetValue("alias", out IStructure? aliasArray) ? aliasArray!.Properties.Select(x => aliasArray[x]).OfType<IString>().Select(x => x.Value).ToArray()
            : [];
        var inside = grammarToken.TryGetValue("inside", out IStructure? insideVar) ? GetGrammar(nestedName, insideVar!, refGrammars, refTokens) : null;

        return new GrammarTokenJson(nestedName, pattern, lookbehind, greedy, alias, inside);
    }

    private static GrammarTokenJson[] GetGrammarTokens(IObject obj, string nestedName, Dictionary<IObject, GrammarJson> refGrammars, Dictionary<IObject, GrammarTokenJson[]> refTokens)
    {
        if (refTokens.TryGetValue(obj, out var tokens))
            return tokens;

        switch (obj)
        {
            // Array
            case IStructure array when array.Properties.All(x => char.IsDigit(x[0])):
                {
                    tokens = new GrammarTokenJson[array.Properties.Count];
                    var i = 0;
                    foreach (var property in array.Properties)
                    {
                        foreach (var token in GetGrammarTokens(array[property], $"{nestedName}_{i}", refGrammars, refTokens))
                        {
                            tokens[i++] = token;
                        }
                    }
                    return refTokens[obj] = tokens;
                }
            // GrammarToken
            case IStructure grammarToken:
                {
                    return refTokens[obj] = new[] { ParseGrammarTokens(nestedName, grammarToken, refGrammars, refTokens) };
                }
            // String
            case IString str:
                {
                    return refTokens[obj] = new[] { new GrammarTokenJson(nestedName, str.Value) };
                }
            default:
                return [];
        }
    }


    private static GrammarJson GetGrammar(string nestedName, IStructure structure, Dictionary<IObject, GrammarJson> refGrammars, Dictionary<IObject, GrammarTokenJson[]> refTokens)
    {
        if (refGrammars.TryGetValue(structure, out var grammar))
            return grammar;
        refGrammars[structure] = grammar = new GrammarJson();

        foreach (var property in structure.Properties)
            grammar[property] = GetGrammarTokens(structure[property], $"{nestedName}_{NormalizeName(property)}", refGrammars, refTokens).ToArray();

        return grammar;
    }
    public static GrammarJson GetGrammar(IStructure structure)
    {
        var refTokens = new Dictionary<IObject, GrammarTokenJson[]>();
        var refGrammars = new Dictionary<IObject, GrammarJson>();

        var grammar = new GrammarJson();
        refGrammars[structure] = grammar;
        foreach (var property in structure.Properties)
            grammar[property] = GetGrammarTokens(structure[property], NormalizeName(property), refGrammars, refTokens).ToArray();
        return grammar;
    }

    public static IEnumerable<KeyValuePair<string, string>> GetPatterns(GrammarTokenJson[] tokens, HashSet<GrammarJson> visited)
    {
        foreach (var token in tokens)
        {
            yield return new(token.PatternVariableName, token.Pattern);

            if (token.Inside is null) continue;
            if (!visited.Add(token.Inside)) continue;

            foreach (var kv in token.Inside)
            {
                foreach (var kv2 in GetPatterns(kv.Value, visited))
                    yield return new(kv2.Key, kv2.Value);
            }
        }
    }

    public static Dictionary<string, string> GetAllRegularExpressions(GrammarJson grammar)
    {
        var dict = new Dictionary<string, string>();
        var visited = new HashSet<GrammarJson>();
        foreach (var kv in grammar)
        {
            foreach (var kv2 in GetPatterns(kv.Value, visited))
            {
                dict[kv2.Key] = kv2.Value;
            }
        }
        return dict;
    }

    public static HashSet<GrammarJson> GetAllUniqueGrammars(GrammarJson grammar)
    {
        var visited = new HashSet<GrammarJson>();
        visited.Add(grammar);
        foreach (var kv in grammar)
            _ = GetPatterns(kv.Value, visited).Count();
        return visited;
    }

    public static HashSet<GrammarTokenJson> GetAllUniqueGrammarTokens(GrammarJson grammar)
    {
        var uniqueGrammars = GetAllUniqueGrammars(grammar);
        var uniqueTokens = new HashSet<GrammarTokenJson>();
        foreach (var uniqueGrammar in uniqueGrammars)
        {
            foreach (var kv in uniqueGrammar)
            {
                foreach (var token in kv.Value)
                    uniqueTokens.Add(token);
            }
        }
        return uniqueTokens;
    }
}
