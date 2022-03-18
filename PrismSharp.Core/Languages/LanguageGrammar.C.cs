using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

public static partial class LanguageGrammar
{
    public static Grammar C => CreateCGrammar();

    private static Grammar CreateCGrammar()
    {
        var extendCLikeGrammar = CLike;

        extendCLikeGrammar["comment"] = new GrammarToken[]
        {
            new(@"\/\/(?:[^\r\n\\]|\\(?:\r\n?|\n|(?![\r\n])))*|\/\*[\s\S]*?(?:\*\/|$)", greedy: true)
        };
        extendCLikeGrammar["char"] = new GrammarToken[]
        {
            // https://en.cppreference.com/w/c/language/character_constant
            new(@"'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n]){0,32}'", greedy: true)
        };
        extendCLikeGrammar["macro"] = new GrammarToken[]
        {
            new(new Regex(@"(^[\t ]*)#\s*[a-z](?:[^\r\n\\/]|\/(?!\*)|\/\*(?:[^*]|\*(?!\/))*\*\/|\\(?:\r\n|[\s\S]))*", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                true,
                true,
                new []{"property"},
                new Grammar
                {
                    ["string"] = new GrammarToken[]
                    {
                        // highlight the path of the include statement as a string
                        new(@"^(#\s*include\s*)<[^>]+>", true)
                    },
                    ["char"] = extendCLikeGrammar["char"],
                    ["comment"] = extendCLikeGrammar["comment"],
                    ["macro-name"] = new GrammarToken[]
                    {
                        new(new Regex(@"(^#\s*define\s+)\w+\b(?!\()", RegexOptions.IgnoreCase), true),
                        new(new Regex(@"(^#\s*define\s+)\w+\b(?=\()", RegexOptions.IgnoreCase), true, alias: new []{"function"}),
                    },
                    ["directive"] = new GrammarToken[]
                    {
                        // highlight macro directives as keywords
                        new(@"^(#\s*)[a-z]+", true, alias: new []{"keyword"})
                    },
                    ["directive-hash"] = new GrammarToken[]{new(@"^#")},
                    ["punctuation"] = new GrammarToken[]{new(@"##|\\(?=[\r\n])")},
                    ["expression"] = new GrammarToken[]
                    {
                        new(@"\S[\s\S]*", inside: extendCLikeGrammar)
                    },
                })
        };
        extendCLikeGrammar["string"] = new GrammarToken[]
        {
            // https://en.cppreference.com/w/c/language/string_literal
            new(@"""(?:\\(?:\r\n|[\s\S])|[^""\\\r\n])*""", greedy: true)
        };
        extendCLikeGrammar["class-name"] = new GrammarToken[]
        {
            new(@"(\b(?:enum|struct)\s+(?:__attribute__\s*\(\([\s\S]*?\)\)\s*)?)\w+|\b[a-z]\w*_t\b", true)
        };
        extendCLikeGrammar["keyword"] = new GrammarToken[]
        {
            new(@"\b(?:_Alignas|_Alignof|_Atomic|_Bool|_Complex|_Generic|_Imaginary|_Noreturn|_Static_assert|_Thread_local|__attribute__|asm|auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|inline|int|long|register|return|short|signed|sizeof|static|struct|switch|typedef|typeof|union|unsigned|void|volatile|while)\b")
        };

        // highlight predefined macros as constants
        extendCLikeGrammar["constant"] = new GrammarToken[]
        {
            new(@"\b(?:EOF|NULL|SEEK_CUR|SEEK_END|SEEK_SET|__DATE__|__FILE__|__LINE__|__TIMESTAMP__|__TIME__|__func__|stderr|stdin|stdout)\b")
        };

        extendCLikeGrammar["function"] = new GrammarToken[]
        {
            new(new Regex(@"\b[a-z_]\w*(?=\s*\()", RegexOptions.IgnoreCase))
        };
        extendCLikeGrammar["number"] = new GrammarToken[]
        {
            new(new Regex(@"(?:\b0x(?:[\da-f]+(?:\.[\da-f]*)?|\.[\da-f]+)(?:p[+-]?\d+)?|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?)[ful]{0,4}", RegexOptions.IgnoreCase))
        };
        extendCLikeGrammar["operator"] = new GrammarToken[]
        {
            new(@">>=?|<<=?|->|([-+&|:])\1|[?:~]|[-+*/%&|^!=<>]=?")
        };

        extendCLikeGrammar.GrammarTokenMap.Remove("boolean");
        return extendCLikeGrammar;
    }
}
