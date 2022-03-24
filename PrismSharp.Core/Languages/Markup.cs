using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-markup.js

public class Markup : IGrammarDefinition
{
    public Grammar Define()
    {
        var markupGrammar = new Grammar
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
                    inside: new Grammar
                    {
                        ["internal-subset"] = new GrammarToken[]
                        {
                            new(@"(^[^\[]*\[)[\s\S]+(?=\]>$)", true, true /* update `inside` later */)
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
                    inside: new Grammar
                    {
                        ["tag"] = new GrammarToken[]
                        {
                            new(@"^<\/?[^\s>\/]+", inside: new Grammar
                            {
                                ["punctuation"] = new GrammarToken[] { new(@"^<\/?") },
                                ["namespace"] = new GrammarToken[] { new(@"^[^\s>\/:]+:") },
                            })
                        },
                        ["special-attr"] = new GrammarToken[] { },
                        ["attr-value"] = new GrammarToken[]
                        {
                            new(@"=\s*(?:""[^""]*""|'[^']*'|[^\s'"">=]+)",
                                inside: new Grammar
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
                            new(@"[^\s>\/]+", inside: new Grammar
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
        };

        markupGrammar["tag"][0].Inside!["attr-value"][0].Inside!["entity"] = markupGrammar["entity"];
        markupGrammar["doctype"][0].Inside!["internal-subset"][0].Inside = markupGrammar;


        // script for js
        var jsGrammar = new JavaScript().Define();
        AddInlined(markupGrammar, "script", jsGrammar, "javascript");

        // add attribute support for all DOM events.
        // https://developer.mozilla.org/en-US/docs/Web/Events#Standard_events
        AddAttribute(markupGrammar,
            @"on(?:abort|blur|change|click|composition(?:end|start|update)|dblclick|error|focus(?:in|out)?|key(?:down|up)|load|mouse(?:down|enter|leave|move|out|over|up)|reset|resize|scroll|select|slotchange|submit|unload|wheel)",
            jsGrammar, "javascript");

        // script runat="server" contains csharp, not javascript
        markupGrammar.InsertBefore("script", new Grammar
        {
            ["asp-script"] = new GrammarToken[]
            {
                new(
                    new Regex(@"(<script(?=.*runat=['""]?server\b)[^>]*>)[\s\S]*?(?=<\/script>)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true, alias: new[] { "asp", "script" }, inside: new CSharp().Define())
            }
        });

        // css style tag
        var cssGrammar = new Css().Define();
        AddInlined(markupGrammar, "style", cssGrammar, "css");
        AddAttribute(markupGrammar, "style", cssGrammar, "css");

        return markupGrammar;
    }

    private static void AddInlined(Grammar grammar, string tagName, Grammar langGrammar, string lang)
    {
        var includedCdataInside = new Grammar
        {
            [$"language-{lang}"] = new GrammarToken[]
            {
                new(new Regex(@"(^<!\[CDATA\[)[\s\S]+?(?=\]\]>$)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true,
                    inside: langGrammar)
            },
            ["cdata"] = new GrammarToken[]
                { new(new Regex(@"^<!\[CDATA\[|\]\]>$", RegexOptions.Compiled | RegexOptions.IgnoreCase)) }
        };

        var inside = new Grammar
        {
            ["included-cdata"] = new GrammarToken[]
            {
                new(new Regex(@"<!\[CDATA\[[\s\S]*?\]\]>", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    inside: includedCdataInside)
            },
            [$"language-{lang}"] = new GrammarToken[]
            {
                new(@"[\s\S]+", inside: langGrammar)
            }
        };

        var def = new Grammar
        {
            [tagName] = new GrammarToken[]
            {
                new(new Regex(@"(<__[^>]*>)(?:<!\[CDATA\[(?:[^\]]|\](?!\]>))*\]\]>|(?!<!\[CDATA\[)[\s\S])*?(?=<\/__>)"
                    .Replace("__", tagName), RegexOptions.Compiled | RegexOptions.IgnoreCase),
                true,
                true,
                inside: inside)
            }
        };

        grammar.InsertBefore("cdata", def);
    }

    private static void AddAttribute(Grammar currentGrammar, string attrNamePattern, Grammar langGrammar, string lang)
    {
        var regex = new Regex(
            $@"(^|[""'\s])(?:{attrNamePattern})\s*=\s*(?:""[^""]*""|'[^']*'|[^\s'"">=]+(?=[\s>]))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        currentGrammar["tag"][0].Inside!["special-attr"] = currentGrammar["tag"][0].Inside!["special-attr"]
            .Append(new GrammarToken(regex, true, inside: new Grammar
            {
                ["attr-name"] = new GrammarToken[]{new(@"^[^\s=]+")},
                ["attr-value"] = new GrammarToken[]{new(@"=[\s\S]+",
                    inside: new Grammar
                    {
                        ["value"] = new GrammarToken[]
                        {
                            new(@"(^=\s*([""']|(?![""'])))\S[\s\S]*(?=\2$)",
                                true,
                                alias: new []{lang, $"language-{lang}"},
                                inside: langGrammar)
                        },
                        ["punctuation"] = new GrammarToken[]
                        {
                            new(@"^=", alias: new []{"attr-equals"}),
                            new(@"""|'")
                        }
                    })},
            })).ToArray();
    }
}
