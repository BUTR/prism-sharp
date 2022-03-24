using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

public class Css : IGrammarDefinition
{
    public Grammar Define()
    {
        const string stringPattern = @"(?:""(?:\\(?:\r\n|[\s\S])|[^""\\\r\n])*""|'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n])*')";

        var cssGrammar = new Grammar
        {
            ["comment"] = new GrammarToken[] { new(@"\/\*[\s\S]*?\*\/") },
            ["atrule"] = new GrammarToken[]
            {
                new(@"@[\w-](?:[^;{\s]|\s+(?![\s{]))*(?:;|(?=\s*\{))",
                    inside: new Grammar
                    {
                        ["rule"] = new GrammarToken[] { new(@"^@[\w-]+") },
                        ["selector-function-argument"] = new GrammarToken[]
                        {
                            new(
                                @"(\bselector\s*\(\s*(?![\s)]))(?:[^()\s]|\s+(?![\s)])|\((?:[^()]|\([^()]*\))*\))+(?=\s*\))",
                                true,
                                alias: new[] { "selector" })
                        },
                        ["keyword"] = new GrammarToken[]
                        {
                            new(@"(^|[^\w-])(?:and|not|only|or)(?![\w-])", true)
                        }
                        // See rest below
                    })
            },
            ["url"] = new GrammarToken[]
            {
                // https://drafts.csswg.org/css-values-3/#urls
                new(new Regex("\\burl\\((?:" + stringPattern + "|" + @"(?:[^\\\r\n()""']|\\[\s\S])*" + ")\\)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    greedy: true,
                    inside: new Grammar
                    {
                        ["function"] = new GrammarToken[]
                            { new(new Regex(@"^url", RegexOptions.Compiled | RegexOptions.IgnoreCase)) },
                        ["punctuation"] = new GrammarToken[] { new(@"^\(|\)$") },
                        ["string"] = new GrammarToken[] { new("^" + stringPattern + "$", alias: new[] { "url" }) },
                    })
            },
            ["selector"] = new GrammarToken[]
            {
                new("(^|[{}\\s])[^{}\\s](?:[^{};\"'\\s]|\\s+(?![\\s{])|" + stringPattern + ")*(?=\\s*\\{)",
                    true)
            },
            ["string"] = new GrammarToken[]
            {
                new(stringPattern, greedy: true)
            },
            ["property"] = new GrammarToken[]
            {
                new(new Regex(@"(^|[^-\w\xA0-\uFFFF])(?!\s)[-_a-z\xA0-\uFFFF](?:(?!\s)[-\w\xA0-\uFFFF])*(?=\s*:)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase), true)
            },
            ["important"] = new GrammarToken[]
            {
                new(new Regex(@"!important\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            },
            ["function"] = new GrammarToken[]
            {
                new(new Regex(@"(^|[^-a-z0-9])[-a-z0-9]+(?=\()",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase), true)
            },
            ["punctuation"] = new GrammarToken[]
            {
                new(@"[(){};:,]")
            }
        };

        cssGrammar["atrule"][0].Inside!.Reset = cssGrammar;
        return cssGrammar;
    }
}
