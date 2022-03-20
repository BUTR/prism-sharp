using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-javascript.js

public class JavaScript : IGrammarDefinition
{
    public Grammar Define()
    {
        var javascriptGrammar = new CLike().Define();
        // var regexGrammar = new RegExp().Define();

        javascriptGrammar["class-name"] = javascriptGrammar["class-name"]
            .Concat(new GrammarToken[]
            {
                new(@"(^|[^$\w\xA0-\uFFFF])(?!\s)[_$A-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\.(?:constructor|prototype))", true)
            }).ToArray();

        javascriptGrammar["keyword"] = new GrammarToken[]
        {
            new(@"((?:^|\})\s*)catch\b", true),
            new(
                @"(^|[^.]|\.\.\.\s*)\b(?:as|assert(?=\s*\{)|async(?=\s*(?:function\b|\(|[$\w\xA0-\uFFFF]|$))|await|break|case|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally(?=\s*(?:\{|$))|for|from(?=\s*(?:['""]|$))|function|(?:get|set)(?=\s*(?:[#\[$\w\xA0-\uFFFF]|$))|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)\b",
                true),
        };

        // Allow for all non-ASCII characters (See http://stackoverflow.com/a/2008444)
        javascriptGrammar["function"] = new GrammarToken[]
        {
            new(@"#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*(?:\.\s*(?:apply|bind|call)\s*)?\()")
        };

        javascriptGrammar["number"] = new GrammarToken[]
        {
            new(@"(^|[^\w$])" + "(?:" + (
                // constant
                "NaN|Infinity" +
                "|" +
                // binary integer
                @"0[bB][01]+(?:_[01]+)*n?" +
                "|" +
                // octal integer
                @"0[oO][0-7]+(?:_[0-7]+)*n?" +
                "|" +
                // hexadecimal integer
                @"0[xX][\dA-Fa-f]+(?:_[\dA-Fa-f]+)*n?" +
                "|" +
                // decimal bigint
                @"\d+(?:_\d+)*n" +
                "|" +
                // decimal number (integer or float) but no bigint
                @"(?:\d+(?:_\d+)*(?:\.(?:\d+(?:_\d+)*)?)?|\.\d+(?:_\d+)*)(?:[Ee][+-]?\d+(?:_\d+)*)?"
            ) + ")" + @"(?![\w$])", true)
        };

        javascriptGrammar["operator"] = new GrammarToken[]
        {
            new(@"--|\+\+|\*\*=?|=>|&&=?|\|\|=?|[!=]==|<<=?|>>>?=?|[-+*/%&|^!=<>]=?|\.{3}|\?\?=?|\?\.?|[~:]")
        };

        javascriptGrammar["class-name"][0].Pattern =
            new Regex(@"(\b(?:class|extends|implements|instanceof|interface|new)\s+)[\w.\\]+", RegexOptions.Compiled);

        javascriptGrammar.InsertBefore("keyword", new Grammar
        {
            ["regex"] = new GrammarToken[]
            {
                new(
                    @"((?:^|[^$\w\xA0-\uFFFF.""'\])\s]|\b(?:return|yield))\s*)\/(?:\[(?:[^\]\\\r\n]|\\.)*\]|\\.|[^/\\\[\r\n])+\/[dgimyus]{0,7}(?=(?:\s|\/\*(?:[^*]|\*(?!\/))*\*\/)*(?:$|[\r\n,.;:})\]]|\/\/))",
                    true,
                    true,
                    inside: new Grammar
                    {
                        ["regex-source"] = new GrammarToken[]
                        {
                            new(@"^(\/)[\s\S]+(?=\/[a-z]*$)",
                                true,
                                alias: new[] { "language-regex" },
                                inside: null/*regexGrammar*/) // test `regex_feature` failed if inside by regex grammar
                        },
                        ["regex-delimiter"] = new GrammarToken[] { new(@"^\/|\/$") },
                        ["regex-flags"] = new GrammarToken[] { new(@"^[a-z]+$") }
                    })
            },

            // This must be declared before keyword because we use "function" inside the look-forward
            ["function-variable"] = new GrammarToken[]
            {
                new(
                    @"#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*[=:]\s*(?:async\s*)?(?:\bfunction\b|(?:\((?:[^()]|\([^()]*\))*\)|(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)\s*=>))",
                    alias: new[] { "function" })
            },
            ["parameter"] = new GrammarToken[]
            {
                new(
                    @"(function(?:\s+(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)?\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\))",
                    true, inside: javascriptGrammar),
                new(
                    new Regex(@"(^|[^$\w\xA0-\uFFFF])(?!\s)[_$a-z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*=>)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true, inside: javascriptGrammar),
                new(@"(\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*=>)",
                    true, inside: javascriptGrammar),
                new(
                    @"((?:\b|\s|^)(?!(?:as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|from|function|get|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|set|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)(?![$\w\xA0-\uFFFF]))(?:(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*\s*)\(\s*|\]\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*\{)",
                    true, inside: javascriptGrammar),
            },
            ["constant"] = new GrammarToken[]
            {
                new(@"\b[A-Z](?:[A-Z_]|\dx?)*\b")
            }
        });

        javascriptGrammar.InsertBefore("string", new Grammar
        {
            ["hashbang"] = new GrammarToken[]
            {
                new(@"^#!.*", greedy: true, alias: new[] { "comment" })
            },
            ["template-string"] = new GrammarToken[]
            {
                new(@"`(?:\\[\s\S]|\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}|(?!\$\{)[^\\`])*`",
                    greedy: true,
                    inside: new Grammar
                    {
                        ["template-punctuation"] = new GrammarToken[]
                        {
                            new(@"^`|`$", alias: new[] { "string" })
                        },
                        ["interpolation"] = new GrammarToken[]
                        {
                            new(@"((?:^|[^\\])(?:\\{2})*)\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}",
                                true,
                                inside: new Grammar
                                {
                                    ["interpolation-punctuation"] = new GrammarToken[]
                                    {
                                        new(@"^\$\{|\}$", alias: new[] { "punctuation" })
                                    },
                                    Reset = javascriptGrammar
                                })
                        },
                        ["string"] = new GrammarToken[]
                        {
                            new(@"[\s\S]+")
                        }
                    })
            },
            ["string-property"] = new GrammarToken[]
            {
                new(
                    new Regex(@"((?:^|[,{])[ \t]*)([""'])(?:\\(?:\r\n|[\s\S])|(?!\2)[^\\\r\n])*\2(?=\s*:)",
                        RegexOptions.Compiled | RegexOptions.Multiline),
                    true,
                    true,
                    new[] { "property" })
            },
        });

        javascriptGrammar.InsertBefore("operator", new Grammar
        {
            ["literal-property"] = new GrammarToken[]
            {
                new(
                    new Regex(@"((?:^|[,{])[ \t]*)(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*:)",
                        RegexOptions.Compiled | RegexOptions.Multiline),
                    true,
                    alias: new[] { "property" })
            }
        });

        return javascriptGrammar;
    }
}
