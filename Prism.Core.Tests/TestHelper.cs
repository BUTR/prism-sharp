using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Prism.Core.Tests;

public static class TestHelper
{
    public static void TestCase(Grammar testGrammar, string code, IReadOnlyList<Token> expected)
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