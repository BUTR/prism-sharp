using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

public class RegExp : IGrammarDefinition
{
    public Grammar Define()
    {
        var specialEscape = new GrammarToken(@"\\[\\(){}[\]^$+*?|.]",
            alias: new[] { "escape" });
        var escape = new GrammarToken(@"\\(?:x[\da-fA-F]{2}|u[\da-fA-F]{4}|u\{[\da-fA-F]+\}|0[0-7]{0,2}|[123][0-7]{2}|c[a-zA-Z]|.)");
        var charSet = new GrammarToken(new Regex(@"\.|\\[wsd]|\\p\{[^{}]+\}", RegexOptions.IgnoreCase),
            alias: new[] { "class-name" });
        var charSetWithoutDot = new GrammarToken(new Regex(@"\\[wsd]|\\p\{[^{}]+\}", RegexOptions.IgnoreCase),
            alias: new[] { "class-name" });
        var rangeChar = @"(?:[^\\\\-]|" + escape.Pattern + ')';
        var range = new Regex(rangeChar + '-' + rangeChar);

        // the name of a capturing group
        var groupName = new GrammarToken(@"(<|')[^<>']+(?=[>']$)", lookbehind:
            true, alias: new[] { "variable" });

        return new Grammar
        {
            ["char-class"] = new GrammarToken[]
            {
                new(@"((?:^|[^\\])(?:\\\\)*)\[(?:[^\\\]]|\\[\s\S])*\]",
                    true,
                    inside: new Grammar
                    {
                        ["char-class-negation"] = new GrammarToken[]
                        {
                            new(@"(^\[)\^", true, alias: new []{"operator"})
                        },
                        ["char-class-punctuation"] = new GrammarToken[]
                        {
                            new(@"^\[|\]$", alias: new []{"punctuation"})
                        },
                        ["range"] = new GrammarToken[]
                        {
                            new(range, inside: new Grammar
                            {
                                ["escape"] = new []{escape},
                                ["range-punctuation"] = new GrammarToken[]
                                {
                                    new(@"-", alias: new []{"operator"})
                                }
                            })
                        },
                        ["special-escape"] = new []{specialEscape},
                        ["char-set'"] = new []{charSetWithoutDot},
                        ["escape"] = new []{escape}
                    })
            },
            ["special-escape"] = new []{specialEscape},
            ["char-set"] = new []{charSet},
            ["backreference"] = new GrammarToken[]
            {
                // a backreference which is not an octal escape
                new(@"\\(?![123][0-7]{2})[1-9]", alias: new []{"keyword"}),
                new(@"\\k<[^<>']+>", alias: new []{"keyword"},
                    inside: new Grammar
                    {
                        ["group-name"] = new []{groupName}
                    }),
            },
            ["anchor"] = new GrammarToken[]
            {
                new(@"[$^]|\\[ABbGZz]", alias: new []{"function"})
            },
            ["escape"] = new []{escape},
            ["group"] = new GrammarToken[]
            {
                // https://docs.oracle.com/javase/10/docs/api/java/util/regex/Pattern.html
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference?view=netframework-4.7.2#grouping-constructs

                // (), (?<name>), (?'name'), (?>), (?:), (?=), (?!), (?<=), (?<!), (?is-m), (?i-m:)
                new(@"\((?:\?(?:<[^<>']+>|'[^<>']+'|[>:]|<?[=!]|[idmnsuxU]+(?:-[idmnsuxU]+)?:?))?",
                    alias: new []{"punctuation"},
                    inside: new Grammar
                    {
                        ["group-name"] = new []{groupName}
                    }),
                new(@"\)", alias: new []{"punctuation"})
            },
            ["quantifier"] = new GrammarToken[]
            {
                new(@"(?:[+*?]|\{\d+(?:,\d*)?\})[?+]?", alias: new []{"number"})
            },
            ["alternation"] = new GrammarToken[]
            {
                new(@"\|", alias: new []{"keyword"})
            }
        };
    }
}
