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
        TestHelper.TestCase(grammar, "// /*\n/* comment */",
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
        TestHelper.TestCase(grammar, "foo \"bar\" 'baz'",
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
        TestHelper.TestCase(grammar, "<'> '' ''\n<\"> \"\" \"\"",
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

}