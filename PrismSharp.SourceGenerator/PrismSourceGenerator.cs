using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace PrismSharp.SourceGenerator;

[Generator]
[ExcludeFromCodeCoverage]
public class PrismSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var languageSourceNames = new List<string>();
        foreach (var languageSource in GenerateLanguageSources())
        {
            languageSourceNames.Add(languageSource.Key);
            context.AddSource($"GrammarProvider.{languageSource.Key}.cs", SourceText.From(languageSource.Value, Encoding.UTF8));
        }

        var sb = new StringBuilder();
        sb.AppendLine($$"""
                        namespace PrismSharp.Core;

                        public static class LanguageGrammars
                        {
                            private static readonly IDictionary<string, Lazy<Grammar>> Definitions;

                            static LanguageGrammars()
                            {
                                Definitions = new Dictionary<string, Lazy<Grammar>>(16);

                        """);
        foreach (var languageSourceName in languageSourceNames)
        {
            sb.AppendLine($$"""
                                    Definitions.Add("{{languageSourceName}}", new Lazy<Grammar>(RegEx.{{languageSourceName}}.GrammarProvider.Create));
                            """);
        }
        sb.AppendLine($$"""
                            }

                            private static Grammar GetGrammar(string language)
                            {
                                Definitions.TryGetValue(language, out var grammar);
                                return grammar?.Value ?? new Grammar();
                            }
                        """);
        foreach (var languageSourceName in languageSourceNames)
        {
            sb.AppendLine($$"""
                                public static Grammar {{languageSourceName}} => GetGrammar("{{languageSourceName}}");
                            """);
        }
        sb.AppendLine($$"""
                        }
                        """);
        context.AddSource("LanguageGrammars.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }


    private static IEnumerable<KeyValuePair<string, string>> GenerateLanguageSources()
    {
        foreach (var languageDefinition in LanguageDeclarationGenerator.GetFlattedLanguageDefinitions())
        {
            var language = languageDefinition.Key;
            var source = languageDefinition.Value;

            var flatted = Flatted.Parse(source)!;
            var grammar = GrammarTokenParser.GetGrammar(flatted);

            var uniqueGrammars = GrammarTokenParser.GetAllUniqueGrammars(grammar).ToArray();
            var uniqueTokens = GrammarTokenParser.GetAllUniqueGrammarTokens(grammar);

            var sb3 = new StringBuilder();
            sb3.AppendLine($$"""
                             using PrismSharp.Core;

                             namespace PrismSharp.RegEx.{{language}};

                             public static class GrammarProvider
                             {
                                 public static Grammar Create()
                                 {
                             """);
            foreach (var uniqueGrammar in uniqueGrammars)
            {
                sb3.AppendLine($$"""
                                         var grammar_{{Array.IndexOf(uniqueGrammars, uniqueGrammar)}} = new Grammar();
                                 """);
            }

            foreach (var uniqueToken in uniqueTokens)
            {
                sb3.AppendLine($$"""
                                         var var_{{uniqueToken.PatternVariableName}} = new GrammarToken(
                                             pattern: new {{uniqueToken.PatternVariableName}}(),
                                             lookbehind: {{(uniqueToken.Lookbehind ? "true" : "false")}},
                                             greedy: {{(uniqueToken.Greedy ? "true" : "false")}},
                                             alias: new string[] { {{string.Join(", ", uniqueToken.Alias.Select(x => $"\"{x}\"") ?? [])}} },
                                             inside: {{(uniqueToken.Inside == null ? "null" : $"grammar_{Array.IndexOf(uniqueGrammars, uniqueToken.Inside)}")}}
                                         );
                                 """);
            }

            foreach (var uniqueGrammar in uniqueGrammars)
            {
                foreach (var kv in uniqueGrammar)
                {
                    sb3.AppendLine($$"""
                                             grammar_{{Array.IndexOf(uniqueGrammars, uniqueGrammar)}}["{{kv.Key}}"] = new GrammarToken[]
                                             {
                                                 {{string.Join(", ", kv.Value.Select(x => $"var_{x.PatternVariableName}"))}}
                                             };
                                     """);
                }
            }

            sb3.AppendLine($$"""
                                     return new Grammar()
                                     {
                             """);
            foreach (var kv in grammar)
            {
                sb3.AppendLine($$"""
                                             ["{{kv.Key}}"] = new GrammarToken[]
                                             {
                                                 {{string.Join(", ", kv.Value.Select(x => $"var_{x.PatternVariableName}"))}}
                                             },
                                 """);
            }

            sb3.AppendLine($$"""
                                     };
                                 }
                             }
                             """);

            yield return new(language, sb3.ToString());
        }
    }
}
