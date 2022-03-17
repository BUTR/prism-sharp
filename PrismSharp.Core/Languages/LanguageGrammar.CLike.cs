using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

public static partial class LanguageGrammar
{
    public static Grammar CLike => new (new Dictionary<string, GrammarToken[]>
    {
        ["comment"] = new GrammarToken[]
        {
            new (@"(^|[^\\])\/\*[\s\S]*?(?:\*\/|$)", true, true),
            new (@"(^|[^\\:])\/\/.*", true, true),
        },
        ["string"] = new GrammarToken[]
        {
            new (@"([""'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1", greedy: true),
        },
        ["class-name"] = new GrammarToken[]
        {
            new (new Regex(@"(\b(?:class|extends|implements|instanceof|interface|new|trait)\s+|\bcatch\s+\()[\w.\\]+", RegexOptions.IgnoreCase),
                true,
                inside: new Dictionary<string, GrammarToken[]>
                {
                    ["punctuation"] = new GrammarToken[]
                    {
                        new (@"[.\\]")
                    }
                }),
        },
        ["keyword"] = new GrammarToken[]
        {
            new (@"\b(?:break|catch|continue|do|else|finally|for|function|if|in|instanceof|new|null|return|throw|try|while)\b"),
        },
        ["boolean"] = new GrammarToken[]
        {
            new (@"\b(?:false|true)\b")
        },
        ["function"] = new GrammarToken[]
        {
            new (@"\b\w+(?=\()"),
        },
        ["number"] = new GrammarToken[]
        {
            new (new Regex(@"\b0x[\da-f]+\b|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?", RegexOptions.IgnoreCase)),
        },
        ["operator"] = new GrammarToken[]
        {
            new (@"[<>]=?|[!=]=?=?|--?|\+\+?|&&?|\|\|?|[?*/~^%]")
        },
        ["punctuation"] = new GrammarToken[]
        {
            new (@"[{}[\];(),.:]")
        }
    });
}
