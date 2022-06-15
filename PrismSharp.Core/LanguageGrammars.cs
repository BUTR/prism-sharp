using PrismSharp.Core.Languages;

namespace PrismSharp.Core;

// TODO: maybe the class could be created by `source-generators`
public static class LanguageGrammars
{
    private static readonly IDictionary<string, Lazy<Grammar>> Definitions;

    static LanguageGrammars()
    {
        Definitions = new Dictionary<string, Lazy<Grammar>>(16);

        AddDefinition<C>("c");
        AddDefinition<CLike>("clike");
        AddDefinition<CSharp>("csharp", "cs", "dotnet");
        AddDefinition<AspNet>("aspnet", "aspx");
        AddDefinition<JavaScript>("javascript", "js");
        AddDefinition<RegExp>("regexp", "regex");
        AddDefinition<Markup>("markup", "html", "mathml", "svg", "xml", "atom", "rss");
        AddDefinition<Sql>("sql");
        AddDefinition<Json>("json", "web-manifest");
        AddDefinition<PowerShell>("powershell", "ps1");
        AddDefinition<Yaml>("yaml", "yml");
        AddDefinition<Css>("css");
        AddDefinition<Lua>("lua");
    }

    public static void AddDefinition<T>(params string[] alias) where T : IGrammarDefinition, new()
    {
        var lazyVal = new Lazy<Grammar>(() => new T().Define());
        foreach (var name in alias)
            Definitions.Add(name, lazyVal);
    }

    public static Grammar GetGrammar(string language)
    {
        Definitions.TryGetValue(language, out var grammar);
        return grammar?.Value ?? new Grammar();
    }

    public static Grammar C => GetGrammar("c");
    public static Grammar CLike => GetGrammar("clike");
    public static Grammar CSharp => GetGrammar("csharp");
    // public static Grammar Cs => CSharp;
    // public static Grammar DotNet => CSharp;
    public static Grammar AspNet => GetGrammar("aspnet");
    // public static Grammar Aspx => AspNet;
    public static Grammar JavaScript => GetGrammar("javascript");
    // public static Grammar Js => JavaScript;
    public static Grammar RegExp => GetGrammar("regexp");
    // public static Grammar Regex => RegExp;
    public static Grammar Markup => GetGrammar("markup");
    // public static Grammar Html => Markup;
    // public static Grammar Mathml => Markup;
    // public static Grammar Svg => Markup;
    // public static Grammar Xml => Markup;
    // public static Grammar Atom => Xml;
    // public static Grammar Rss => Xml;
    public static Grammar Sql => GetGrammar("sql");
    public static Grammar Json => GetGrammar("json");
    // public static Grammar WebManifest => Json;
    public static Grammar PowerShell => GetGrammar("powershell");
    // public static Grammar Ps1 => PowerShell;
    public static Grammar Yaml => GetGrammar("yaml");
    // public static Grammar Yml => Yaml;
    public static Grammar Css => GetGrammar("css");
    public static Grammar Lua => GetGrammar("lua");
}
