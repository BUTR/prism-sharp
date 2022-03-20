using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-json.js

public class Json : IGrammarDefinition
{
    public Grammar Define()
    {
        return new Grammar
        {
            ["property"] = new GrammarToken[]
            {
                new(@"(^|[^\\])""(?:\\.|[^\\""\r\n])*""(?=\s*:)", true, true)
            },
            ["string"] = new GrammarToken[]
            {
                new(@"(^|[^\\])""(?:\\.|[^\\""\r\n])*""(?!\s*:)", true, true)
            },
            ["comment"] = new GrammarToken[]
            {
                new(@"\/\/.*|\/\*[\s\S]*?(?:\*\/|$)", greedy: true)
            },
            ["number"] = new GrammarToken[]
            {
                new(new Regex(@"-?\b\d+(?:\.\d+)?(?:e[+-]?\d+)?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            },
            ["punctuation"] = new GrammarToken[] { new(@"[{}[\],]") },
            ["operator"] = new GrammarToken[] { new(@":") },
            ["boolean"] = new GrammarToken[] { new(@"\b(?:false|true)\b") },
            ["null"] = new GrammarToken[]
            {
                new(@"\bnull\b", alias: new[] { "keyword" })
            },
        };
    }
}
