using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace PrismSharp.Core.Tests;

public class PrismTest
{
    [Fact]
    public void Tokenize_greedy_Ok()
    {
        var grammar = new Grammar
        {
            ["comment"] = new GrammarToken[]
            {
                new(@"\/\/.*"),
                new(@"\/\*[\s\S]*?(?:\*\/|$)", greedy: true)
            }
        };
        TestHelper.RunTestCase(grammar, "// /*\n/* comment */",
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
        var grammar = new Grammar
        {
            ["a"] = new GrammarToken[]
            {
                new(@"'[^']*'"),
            },
            ["b"] = new GrammarToken[]
            {
                new(@"foo|(^|[^\\])""[^""]*""", true, true)
            }
        };
        TestHelper.RunTestCase(grammar, "foo \"bar\" 'baz'",
            new StringToken[]
            {
                new("foo", "b"),
                new("\"bar\"", "b"),
                new("'baz'", "a"),
            });
    }

    [Fact]
    public void Tokenize_should_correctly_rematch_tokens()
    {
        var grammar = new Grammar
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
        };
        TestHelper.RunTestCase(grammar, "<'> '' ''\n<\"> \"\" \"\"",
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

    [Fact]
    public void Tokenize_should_always_match_tokens_against_the_whole_text()
    {
        var grammar = new Grammar
        {
            ["a"] = new GrammarToken[] { new("a"), },
            ["b"] = new GrammarToken[] { new("^b", greedy: true) },
        };
        TestHelper.RunTestCase(grammar, "bab",
            new StringToken[]
            {
                new("b", "b"),
                new("a", "a"),
                new("b"),
            });
    }

    [Fact]
    public void Tokenize_from_prismjs_issue3052()
    {
        // If a greedy pattern creates an empty token at the end of the string, then this token should be discarded
        var grammar = new Grammar
        {
            ["oh-no"] = new GrammarToken[] { new("$", greedy: true) },
        };
        TestHelper.RunTestCase(grammar, "foo",
            new StringToken[]
            {
                new("foo"),
            });
    }

}
