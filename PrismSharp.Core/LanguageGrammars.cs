using PrismSharp.Core.Languages;

namespace PrismSharp.Core;

// TODO: maybe the class could be created by `source-generators`
public static class LanguageGrammars
{
    public static Grammar C => new C().Define();
    public static Grammar CLike => new CLike().Define();
    public static Grammar CSharp => new CSharp().Define();
    public static Grammar Cs => CSharp;
    public static Grammar DotNet => CSharp;
    public static Grammar AspNet => new AspNet().Define();
    public static Grammar Aspx => AspNet;
    public static Grammar JavaScript => new JavaScript().Define();
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
}
