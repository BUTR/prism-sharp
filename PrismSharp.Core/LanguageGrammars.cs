using PrismSharp.Core.Languages;

namespace PrismSharp.Core;

// TODO: maybe the class could be created by `source-generators`
public static class LanguageGrammars
{
    private static readonly IReadOnlyDictionary<string, Lazy<Grammar>> Definitions;

    static LanguageGrammars()
    {
        Definitions = new Dictionary<string, Lazy<Grammar>>
        {
            ["csharp"] = new(() => new CSharp().Define()),
            ["javascript"] = new(() => new JavaScript().Define()),
        };
    }


    public static Grammar C => new C().Define();
    public static Grammar Cpp => Definitions["cpp"].Value;
    public static Grammar CLike => new CLike().Define();
    public static Grammar CSharp => Definitions["csharp"].Value;
    public static Grammar Cs => CSharp;
    public static Grammar DotNet => CSharp;
    public static Grammar AspNet => new AspNet().Define();
    public static Grammar Aspx => AspNet;
    public static Grammar JavaScript => Definitions["javascript"].Value;
    public static Grammar Js => JavaScript;
    public static Grammar RegExp => new RegExp().Define();
    public static Grammar Regex => RegExp;
    public static Grammar Markup => new Markup().Define();
    public static Grammar Html => Markup;
    public static Grammar Mathml => Markup;
    public static Grammar Svg => Markup;
    public static Grammar Xml => Markup;
    public static Grammar Atom => Xml;
    public static Grammar Rss => Xml;
    public static Grammar Sql => new Sql().Define();
    public static Grammar Json => new Json().Define();
    public static Grammar WebManifest => Json;
    public static Grammar PowerShell = new PowerShell().Define();
    public static Grammar Ps1 => PowerShell;
    public static Grammar Yaml => new Yaml().Define();
    public static Grammar Yml => Yaml;
    public static Grammar Css => new Css().Define();
}
