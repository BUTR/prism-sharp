using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PrismSharp.Core.Tests;

public static class TestHelper
{
    public static void RunTestCase(Grammar testGrammar, string code, IReadOnlyList<Token> expected)
    {
        var tokens = Prism.Tokenize(code, testGrammar);
        var simpleTokens = Simplify(tokens);
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

    private static Token[] Simplify(IReadOnlyCollection<Token> tokens)
    {
        return tokens
            .Where(t => !IsBlankStringToken(t))
            .Select(InnerSimplify)
            .ToArray();
    }

    private static Token InnerSimplify(Token token)
    {
        return token switch
        {
            StringToken stringToken => new StringToken(stringToken.Content, stringToken.Type),
            StreamToken streamToken => new StreamToken(Simplify(streamToken.Content), streamToken.Type),
            _ => throw new ArgumentException("Type is not support!", nameof(token))
        };
    }

    private static bool IsBlankStringToken(Token token)
    {
        return token is StringToken stringToken && string.IsNullOrWhiteSpace(stringToken.Content);
    }
}
