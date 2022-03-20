using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-csharp.js

public class CSharp : IGrammarDefinition
{
    public Grammar Define()
    {
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
        var keywordKinds = new
        {
            // keywords which represent a return or variable type
            type =
                "bool byte char decimal double dynamic float int long object sbyte short string uint ulong ushort var void",
            // keywords which are used to declare a type
            typeDeclaration = "class enum interface record struct",
            // contextual keywords
            // ("var" and "dynamic" are missing because they are used like types)
            contextual =
                "add alias and ascending async await by descending from(?=\\s*(?:\\w|$)) get global group into init(?=\\s*;) join let nameof not notnull on or orderby partial remove select set unmanaged value when where with(?=\\s*{)",
            // all other keywords
            other =
                "abstract as base break case catch checked const continue default delegate do else event explicit extern finally fixed for foreach goto if implicit in internal is lock namespace new null operator out override params private protected public readonly ref return sealed sizeof stackalloc static switch this throw try typeof unchecked unsafe using virtual volatile while yield"
        };

        var typeDeclarationKeywords = KeywordsToPattern(keywordKinds.typeDeclaration);
        var keywords =
            new Regex(
                KeywordsToPattern(
                    $"{keywordKinds.type} {keywordKinds.typeDeclaration} {keywordKinds.contextual} {keywordKinds.other}"),
                RegexOptions.Compiled);
        var nonTypeKeywords = KeywordsToPattern(
            $"{keywordKinds.typeDeclaration} {keywordKinds.contextual} {keywordKinds.other}");
        var nonContextualKeywords = KeywordsToPattern(
            $"{keywordKinds.type} {keywordKinds.typeDeclaration} {keywordKinds.other}");

        // types
        var generic = Nested(@"<(?:[^<>;=+\-*/%&|^]|<<self>>)*>", 2); // the idea behind the other forbidden characters is to prevent false positives. Same for tupleElement.
        var nestedRound = Nested(@"\((?:[^()]|<<self>>)*\)", 2);
        const string name = @"@?\b[A-Za-z_]\w*\b";
        var genericName = Replace(@"<<0>>(?:\s*<<1>>)?", new[] { name, generic });
        var identifier = Replace(@"(?!<<0>>)<<1>>(?:\s*\.\s*<<1>>)*", new[] { nonTypeKeywords, genericName });
        const string array = @"\[\s*(?:,\s*)*\]";
        var typeExpressionWithoutTuple =
            Replace(@"<<0>>(?:\s*(?:\?\s*)?<<1>>)*(?:\s*\?)?", new[] { identifier, array });
        var tupleElement = Replace(@"[^,()<>[\];=+\-*/%&|^]|<<0>>|<<1>>|<<2>>", new[] { generic, nestedRound, array });
        var tuple = Replace(@"\(<<0>>+(?:,<<0>>+)+\)", new[] { tupleElement });
        var typeExpression = Replace(@"(?:<<0>>|<<1>>)(?:\s*(?:\?\s*)?<<2>>)*(?:\s*\?)?",
            new[] { tuple, identifier, array });

        var typeInside = new Grammar
        {
            ["keyword"] = new GrammarToken[] { new(keywords) },
            ["punctuation"] = new GrammarToken[] { new(@"[<>()?,.:[\]]") }
        };

        // strings & characters
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#character-literals
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#string-literals
        const string character = @"'(?:[^\r\n'\\]|\\.|\\[Uux][\da-fA-F]{1,8})'"; // simplified pattern
        const string regularString = @"""(?:\\.|[^\\""\r\n])*""";
        const string verbatimString = @"@""(?:""""|\\[\s\S]|[^\\""])*""(?!"")";

        var csharpGrammar = new CLike().Define();

        csharpGrammar["string"] = new GrammarToken[]
        {
            new(Re(@"(^|[^$\\])<<0>>", new[] { verbatimString }), true, true),
            new(Re(@"(^|[^@$\\])<<0>>", new[] { regularString }), true, true),
        };
        csharpGrammar["class-name"] = new GrammarToken[]
        {
            // Using static
            // using static System.Math;
            new(Re(@"(\busing\s+static\s+)<<0>>(?=\s*;)", new[] { identifier }), true, inside: typeInside),
            // Using alias (type)
            // using Project = PC.MyCompany.Project;
            new(Re(@"(\busing\s+<<0>>\s*=\s*)<<1>>(?=\s*;)", new[] { name, typeExpression }), true, inside: typeInside),
            // Using alias (type)
            // using Project = PC.MyCompany.Project;
            new(Re(@"(\busing\s+)<<0>>(?=\s*=)", new[] { name }), true),
            // Type declarations
            // class Foo<A, B>
            // interface Foo<out A, B>
            new(Re(@"(\b<<0>>\s+)<<1>>", new[] { typeDeclarationKeywords, genericName }), true, inside: typeInside),
            // Single catch exception declaration
            // catch(Foo)
            // (things like catch(Foo e) is covered by variable declaration)
            new(Re(@"(\bcatch\s*\(\s*)<<0>>", new[] { identifier }), true, inside: typeInside),
            // Name of the type parameter of generic constraints
            // where Foo : class
            new(Re(@"(\bwhere\s+)<<0>>", new[] { name }), true),
            // Casts and checks via as and is.
            // as Foo<A>, is Bar<B>
            // (things like if(a is Foo b) is covered by variable declaration)
            new(Re(@"(\b(?:is(?:\s+not)?|as)\s+)<<0>>", new[] { typeExpressionWithoutTuple }), true,
                inside: typeInside),
            // Variable, field and parameter declaration
            // (Foo bar, Bar baz, Foo[,,] bay, Foo<Bar, FooBar<Bar>> bax)
            new(
                Re(@"\b<<0>>(?=\s+(?!<<1>>|with\s*\{)<<2>>(?:\s*[=,;:{)\]]|\s+(?:in|when)\b))",
                    new[] { typeExpression, nonContextualKeywords, name }), inside: typeInside),
        };
        csharpGrammar["keyword"] = new GrammarToken[] { new(keywords) };

        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#literals
        csharpGrammar["number"] = new GrammarToken[]
        {
            new(new Regex(
                @"(?:\b0(?:x[\da-f_]*[\da-f]|b[01_]*[01])|(?:\B\.\d+(?:_+\d+)*|\b\d+(?:_+\d+)*(?:\.\d+(?:_+\d+)*)?)(?:e[-+]?\d+(?:_+\d+)*)?)(?:[dflmu]|lu|ul)?\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase))
        };
        csharpGrammar["operator"] = new GrammarToken[]
        {
            new(@">>=?|<<=?|[-=]>|([-+&|])\1|~|\?\?=?|[-+*/%&|^!=<>]=?")
        };
        csharpGrammar["punctuation"] = new GrammarToken[]
        {
            new(@"\?\.?|::|[{}[\];(),.:]")
        };

        csharpGrammar.InsertBefore("number", new Grammar
        {
            ["range"] = new GrammarToken[]
            {
                new(@"\.\.", alias: new[] { "operator" })
            }
        });
        csharpGrammar.InsertBefore("punctuation", new Grammar
        {
            ["named-parameter"] = new GrammarToken[]
            {
                new(Re(@"([(,]\s*)<<0>>(?=\s*:)", new[] { name }), true, alias: new[] { "punctuation" })
            }
        });

        csharpGrammar.InsertBefore("class-name", new Grammar
        {
            ["namespace"] = new GrammarToken[]
            {
                // namespace Foo.Bar {}
                // using Foo.Bar;
                new(Re(@"(\b(?:namespace|using)\s+)<<0>>(?:\s*\.\s*<<0>>)*(?=\s*[;{])", new []{name}),
                    true,
                    inside: new Grammar
                    {
                        ["punctuation"] = new GrammarToken[]{new(@"\.")}
                    })
            },
            ["type-expression"] = new GrammarToken[]
            {
                // default(Foo), typeof(Foo<Bar>), sizeof(int)
                new(Re(@"(\b(?:default|sizeof|typeof)\s*\(\s*(?!\s))(?:[^()\s]|\s(?!\s)|<<0>>)*(?=\s*\))", new []{nestedRound}),
                    true,
                    alias: new []{"class-name"},
                    inside: typeInside)
            },
            ["return-type"] = new GrammarToken[]
            {
                // Foo<Bar> ForBar(); Foo IFoo.Bar() => 0
                // int this[int index] => 0; T IReadOnlyList<T>.this[int index] => this[index];
                // int Foo => 0; int Foo { get; set } = 0;
                new(Re(@"<<0>>(?=\s+(?:<<1>>\s*(?:=>|[({]|\.\s*this\s*\[)|this\s*\[))", new []{typeExpression, identifier}),
                    alias: new []{"class-name"},
                    inside: typeInside)
            },
            ["constructor-invocation"] = new GrammarToken[]
            {
                // new List<Foo<Bar[]>> { }
                new(Re(@"(\bnew\s+)<<0>>(?=\s*[[({])", new []{typeExpression}),
                    true,
                    alias: new []{"class-name"},
                    inside: typeInside)
            },
            // TODO： explicit-implementation
            ["generic-method"] = new GrammarToken[]
            {
                // new List<Foo<Bar[]>> { }
                new(Re(@"<<0>>\s*<<1>>(?=\s*\()", new []{name, generic}),
                    inside: new Grammar
                    {
                        ["function"] = new GrammarToken[]{new(Re(@"^<<0>>", new []{name}))},
                        ["generic"] = new GrammarToken[]
                        {
                            new(new Regex(generic, RegexOptions.Compiled), alias: new []{"class-name"}, inside: typeInside)
                        },
                    })
            },
            ["type-list"] = new GrammarToken[]
            {
                // The list of types inherited or of generic constraints
                // class Foo<F> : Bar, IList<FooBar>
                // where F : Bar, IList<int>
                new(
                    Re(@"\b((?:<<0>>\s+<<1>>|record\s+<<1>>\s*<<5>>|where\s+<<2>>)\s*:\s*)(?:<<3>>|<<4>>|<<1>>\s*<<5>>|<<6>>)(?:\s*,\s*(?:<<3>>|<<4>>|<<6>>))*(?=\s*(?:where|[{;]|=>|$))",
                    new []{typeDeclarationKeywords, genericName, name, typeExpression, keywords.ToString(), nestedRound, @"\bnew\s*\(\s*\)"}),
                    true,
                    inside: new Grammar
                    {
                        ["record-arguments"] = new GrammarToken[]
                        {
                            new(Re(@"(^(?!new\s*\()<<0>>\s*)<<1>>", new []{genericName, nestedRound}),
                                true, true, inside: csharpGrammar)
                        },
                        ["keyword"] = new GrammarToken[]{new(keywords)},
                        ["class-name"] = new GrammarToken[]{new(typeExpression, greedy: true, inside: typeInside)},
                        ["punctuation"] = new GrammarToken[]{new(@"[,()]")}
                    })
            },
            ["preprocessor"] = new GrammarToken[]
            {
                new(new Regex(@"(^[\t ]*)#.*", RegexOptions.Compiled | RegexOptions.Multiline),
                    true,
                    alias: new []{"property"},
                    inside: new Grammar
                    {
                        ["directive"] = new GrammarToken[]
                        {
                            // highlight preprocessor directives as keywords
                            new(@"(#)\b(?:define|elif|else|endif|endregion|error|if|line|nullable|pragma|region|undef|warning)\b",
                                true, alias: new []{"keyword"})
                        }
                    })
            }
        });

        // attributes
        const string regularStringOrCharacter = $"{regularString}|{character}";
        var regularStringCharacterOrComment = Replace(@"\/(?![*/])|\/\/[^\r\n]*[\r\n]|\/\*(?:[^*]|\*(?!\/))*\*\/|<<0>>",
            new[] { regularStringOrCharacter });
        var roundExpression =
            Nested(Replace(@"[^""'/()]|<<0>>|\(<<self>>*\)", new[] { regularStringCharacterOrComment }), 2);


        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/#attribute-targets
        const string attrTarget = @"\b(?:assembly|event|field|method|module|param|property|return|type)\b";
        var attr = Replace(@"<<0>>(?:\s*\(<<1>>*\))?", new[] { identifier, roundExpression });

        csharpGrammar.InsertBefore("class-name", new Grammar
        {
            ["attribute"] = new GrammarToken[]
            {
                // Attributes
                // [Foo], [Foo(1), Bar(2, Prop = "foo")], [return: Foo(1), Bar(2)], [assembly: Foo(Bar)]
                new(
                    Re(@"((?:^|[^\s\w>)?])\s*\[\s*)(?:<<0>>\s*:\s*)?<<1>>(?:\s*,\s*<<1>>)*(?=\s*\])",
                        new[] { attrTarget, attr }),
                    true,
                    true,
                    inside: new Grammar
                    {
                        ["target"] = new GrammarToken[]
                        {
                            new(Re(@"^<<0>>(?=\s*:)", new[] { attrTarget }), alias: new[] { "keyword" })
                        },
                        ["attribute-arguments"] = new GrammarToken[]
                        {
                            new(Re(@"\(<<0>>*\)", new[] { roundExpression }), inside: csharpGrammar)
                        },
                        ["class-name"] = new GrammarToken[]
                        {
                            new(identifier, inside: new Grammar
                            {
                                ["punctuation"] = new GrammarToken[] { new(@"\.") }
                            })
                        },
                        ["punctuation"] = new GrammarToken[] { new(@"[:,]") }
                    })
            }
        });


        // string interpolation
        var formatString = @":[^}\r\n]+";
        // multi line
        var mInterpolationRound =
            Nested(Replace(@"[^""'/()]|<<0>>|\(<<self>>*\)", new[] { regularStringCharacterOrComment }), 2);
        var mInterpolation =
            Replace(@"\{(?!\{)(?:(?![}:])<<0>>)*<<1>>?\}", new[] { mInterpolationRound, formatString });
        // single line
        var sInterpolationRound =
            Nested(
                Replace(@"[^""'/()]|\/(?!\*)|\/\*(?:[^*]|\*(?!\/))*\*\/|<<0>>|\(<<self>>*\)",
                    new[] { regularStringOrCharacter }), 2);
        var sInterpolation =
            Replace(@"\{(?!\{)(?:(?![}:])<<0>>)*<<1>>?\}", new[] { sInterpolationRound, formatString });


        csharpGrammar.InsertBefore("string", new Grammar
        {
            ["interpolation-string"] = new GrammarToken[]
            {
                new(Re(@"(^|[^\\])(?:\$@|@\$)""(?:""""|\\[\s\S]|\{\{|<<0>>|[^\\{""])*""", new[] { mInterpolation }),
                    true,
                    true,
                    inside: CreateInterpolationInside(mInterpolation, mInterpolationRound, formatString,
                        csharpGrammar)),
                new(Re(@"(^|[^@\\])\$""(?:\\.|\{\{|<<0>>|[^\\""{])*""", new[] { sInterpolation }),
                    true,
                    true,
                    inside: CreateInterpolationInside(sInterpolation, sInterpolationRound, formatString,
                        csharpGrammar)),

            },
            ["char"] = new GrammarToken[] { new(character, greedy: true) },
        });


       return csharpGrammar;
    }

    private static Grammar CreateInterpolationInside(string interpolation, string interpolationRound, string formatString, Grammar csharp)
    {
        return new Grammar
        {
            ["interpolation"] = new GrammarToken[]
            {
                new(Re(@"((?:^|[^{])(?:\{\{)*)<<0>>", new[] { interpolation }),
                    true,
                    inside: new Grammar
                    {
                        ["format-string"] = new GrammarToken[]
                        {
                            new(Re(@"(^\{(?:(?![}:])<<0>>)*)<<1>>(?=\}$)", new[] { interpolationRound, formatString }),
                                true,
                                inside: new Grammar
                                {
                                    ["punctuation"] = new GrammarToken[] { new(@"^:") }
                                })
                        },
                        ["punctuation"] = new GrammarToken[] { new(@"^\{|\}$") },
                        ["expression"] = new GrammarToken[]
                        {
                            new(@"[\s\S]+", alias: new[] { "language-csharp" }, inside: csharp)
                        }
                    }
                ),
            },
            ["string"] = new GrammarToken[] { new(@"[\s\S]+") }
        };
    }

    // keywords
    private static string KeywordsToPattern(string words)
    {
        return "\\b(?:" + words.Trim().Replace(" ", "|") + ")\\b";
    }

    /// <summary>
    /// Creates a nested pattern where all occurrences of the string `&lt;&lt;self>>` are replaced with the pattern itself.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="depthLog2"></param>
    /// <returns></returns>
    private static string Nested(string pattern, int depthLog2)
    {
        var selfRegex = new Regex(@"<<self>>");

        for (var i = 0; i < depthLog2; i++)
        {
            pattern = selfRegex.Replace(pattern, $"(?:{pattern})");
        }

        return selfRegex.Replace(pattern, @"[^\\s\\S]");
    }

    private static Regex Re(string pattern, string[] replacements, RegexOptions flags = RegexOptions.Compiled)
    {
        return new Regex(Replace(pattern, replacements), flags);
    }

    /// <summary>
    /// Replaces all placeholders "&lt;&lt;n>>" of given pattern with the n-th replacement (zero based).
    /// Note: This is a simple text based replacement. Be careful when using backreferences!
    /// <example>
    ///     Replace(@"a&lt;&lt;0>>a", new []{ @"b+" }) == @"a(?:b+)a"
    /// </example>
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replacements"></param>
    /// <returns></returns>
    private static string Replace(string pattern, string[] replacements)
    {
        var evaluator = new MatchEvaluator(match =>
        {
            var idx = int.Parse(match.Groups[1].Value);
            return $"(?:{replacements[idx]})";
        });
        return new Regex(@"<<(\d+)>>", RegexOptions.Compiled).Replace(pattern, evaluator);
    }

}
