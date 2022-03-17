using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace Prism.Core.Tests;

public class PrismTest
{
    [Fact]
    public void Tokenize_greedy_Ok()
    {
        var grammar = new Grammar(new Dictionary<string, GrammarToken[]>
        {
            ["comment"] = new GrammarToken[]
            {
                new(@"\/\/.*"),
                new(@"\/\*[\s\S]*?(?:\*\/|$)", greedy: true)
            }
        });
        TestCase(grammar, "// /*\n/* comment */",
            new StringToken[]
            {
                new("// /*", "comment"),
                // new("\n"),
                new("/* comment */", "comment"),
            });
    }
    
    [Fact]
    public void Tokenize_greedy_lookbehind_Ok()
    {
        var grammar = new Grammar(new Dictionary<string, GrammarToken[]>
        {
            ["a"] = new GrammarToken[]
            {
                new(@"'[^']*'"),
            },
            ["b"] = new GrammarToken[]
            {
                new(@"foo|(^|[^\\])""[^""]*""", true, true)
            }
        });
        TestCase(grammar, "foo \"bar\" 'baz'",
            new StringToken[]
            {
                new("foo", "b"),
                new("\"bar\"", "b"),
                new("'baz'", "a"),
            });
    }
    
    [Fact]
    public void Tokenize_rematch_Ok()
    {
        var grammar = new Grammar(new Dictionary<string, GrammarToken[]>
        {
            ["a"] = new GrammarToken[]
            {
                new(@"'[^'\r\n]*'"),
            },
            ["b"] = new GrammarToken[]
            {
                new(@"""[^""\r\n]*""", greedy: true)
            },
            ["c"] = new GrammarToken[]
            {
                new(@"<[^>\r\n]*>", greedy: true)
            }
        });
        TestCase(grammar, "<'> '' ''\n<\"> \"\" \"\"",
            new StringToken[]
            {
                new("<'>", "c"),
                new(" '"),
                new("' '", "a"),
                new("'\n"),
                new("<\">", "c"),
                new("\"\"", "b"),
                new("\"\"", "b"),
            });
    }

    private static void TestCase(Grammar testGrammar, string code, Token[] expected)
    {
        var tokens = Prism.Tokenize(code, testGrammar);
        var simpleTokens = tokens.Where(t => !IsBlankStringToken(t)).ToArray();
        AssertDeepStrictEqual(simpleTokens, expected);
    }

    private static void AssertDeepStrictEqual(IReadOnlyList<Token> simpleTokens, IReadOnlyList<Token> expected)
    {
        Assert.NotNull(simpleTokens);
        Assert.Equal(expected.Count, simpleTokens.Count);

        for (var i = 0; i < expected.Count; i++)
        {
            var token = simpleTokens[i];
            var expectedToken = expected[i];
            
            if (expectedToken is StringToken expectedStringToken)
            {
                var stringToken = Assert.IsType<StringToken>(token);
                Assert.Equal(expectedStringToken.Type, stringToken.Type);
                Assert.Equal(expectedStringToken.Content, stringToken.Content);
                continue;
            }

            if (expectedToken is not StreamToken expectedStreamToken) 
                continue;
            
            var streamToken = Assert.IsType<StreamToken>(token);
            AssertDeepStrictEqual(streamToken.Content, expectedStreamToken.Content);
        }
    }

    private static bool IsBlankStringToken(Token token)
    {
        return token is StringToken stringToken && string.IsNullOrWhiteSpace(stringToken.Content);
    }

}