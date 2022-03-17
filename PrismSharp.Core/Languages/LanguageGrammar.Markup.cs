using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

public static partial class LanguageGrammar
{
    public static Grammar Markup => CreateMarkupGrammar();

    private static Grammar CreateMarkupGrammar()
    {
        var markupGrammar = new Grammar(new Dictionary<string, GrammarToken[]>
        {
            ["comment"] = new GrammarToken[]
            {
                new(@"<!--(?:(?!<!--)[\s\S])*?-->", greedy: true)
            },
            ["prolog"] = new GrammarToken[]
            {
                new(@"<\?[\s\S]+?\?>", greedy: true)
            },
            ["doctype"] = new GrammarToken[]
            {
                // https://www.w3.org/TR/xml/#NT-doctypedecl
                new(
                    new Regex(
                        @"<!DOCTYPE(?:[^>""'[\]]|""[^""]*""|'[^']*')+(?:\[(?:[^<""'\]]|""[^""]*""|'[^']*'|<(?!!--)|<!--(?:[^-]|-(?!->))*-->)*\]\s*)?>",
                        RegexOptions.IgnoreCase),
                    greedy: true,
                    inside: new Dictionary<string, GrammarToken[]>
                    {
                        ["internal-subset"] = new GrammarToken[]
                        {
                            new(@"(^[^\[]*\[)[\s\S]+(?=\]>$)", true, true
                                // TODO: inside
                            )
                        },
                        ["string"] = new GrammarToken[]
                        {
                            new("\"[^\"]*\"|'[^']*'", greedy: true)
                        },
                        ["punctuation"] = new GrammarToken[]
                        {
                            new(@"^<!|>$|[[\]]")
                        },
                        ["doctype-tag"] = new GrammarToken[]
                        {
                            new(new Regex(@"^DOCTYPE", RegexOptions.IgnoreCase))
                        },
                        ["name"] = new GrammarToken[]
                        {
                            new(@"[^\s<>'""]+")
                        }
                    })
            },
            ["cdata"] = new GrammarToken[]
            {
                new(new Regex(@"<!\[CDATA\[[\s\S]*?\]\]>", RegexOptions.IgnoreCase), greedy: true)
            },
            ["tag"] = new GrammarToken[]
            {
                new(
                    @"<\/?(?!\d)[^\s>\/=$<%]+(?:\s(?:\s*[^\s>\/=]+(?:\s*=\s*(?:""[^""]*""|'[^']*'|[^\s'"">=]+(?=[\s>]))|(?=[\s/>])))+)?\s*\/?>",
                    greedy: true,
                    inside: new Dictionary<string, GrammarToken[]>
                    {
                        ["tag"] = new GrammarToken[]
                        {
                            new(@"^<\/?[^\s>\/]+", inside: new Dictionary<string, GrammarToken[]>
                            {
                                ["punctuation"] = new GrammarToken[] { new(@"^<\/?") },
                                ["namespace"] = new GrammarToken[] { new(@"^[^\s>\/:]+:") },
                            })
                        },
                        ["special-attr"] = new GrammarToken[] { },
                        ["attr-value"] = new GrammarToken[]
                        {
                            new(@"=\s*(?:""[^""]*""|'[^']*'|[^\s'"">=]+)",
                                inside: new Dictionary<string, GrammarToken[]>
                                {
                                    ["punctuation"] = new GrammarToken[]
                                    {
                                        new(@"^=", alias: new[] { "attr-equals" }),
                                        new("\"|'"),
                                    }
                                })
                        },
                        ["punctuation"] = new GrammarToken[] { new(@"\/?>") },
                        ["attr-name"] = new GrammarToken[]
                        {
                            new(@"[^\s>\/]+", inside: new Dictionary<string, GrammarToken[]>
                            {
                                ["namespace"] = new GrammarToken[] { new(@"^[^\s>\/:]+:") }
                            })
                        }
                    })
            },
            ["entity"] = new GrammarToken[]
            {
                new(new Regex(@"&[\da-z]{1,8};", RegexOptions.IgnoreCase), alias: new[] { "named-entity" }),
                new(new Regex(@"&#x?[\da-f]{1,8};", RegexOptions.IgnoreCase))
            }
        });

        markupGrammar["tag"][0].Inside!["attr-value"][0].Inside!["entity"] = markupGrammar["entity"];
        markupGrammar["doctype"][0].Inside!["internal-subset"][0].Inside = markupGrammar;

        return markupGrammar;
    }

    public static Grammar Html => Markup;
    public static Grammar Mathml => Markup;
    public static Grammar Svg => Markup;

    // TODO
    // public static Grammar Xml => Markup;
    // public static Grammar Atom => Xml;
    // public static Grammar Rss => Xml;
}