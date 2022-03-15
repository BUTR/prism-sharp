using System.Collections.Generic;
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
        var tokens = Prism.Tokenize(@"// /*\n/* comment */", grammar);
        var expected = new[]
        {
            ("comment", "// /*"),
            ("comment", "/* comment */"),
        };
        Assert.NotNull(tokens);
        Assert.Equal(expected.Length, tokens.Length);

        for (var i = 0; i < expected.Length; i++)
        {
            var token = tokens[i];
            var stringToken = Assert.IsType<StringToken>(token);
            Assert.Equal(expected[i].Item1, stringToken.Type);
            Assert.Equal(expected[i].Item2, stringToken.Content);
        }
    }
}