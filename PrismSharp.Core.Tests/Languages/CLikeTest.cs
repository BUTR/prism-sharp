using PrismSharp.Core.Languages;
using Xunit;

namespace PrismSharp.Core.Tests.Languages;

public class CLikeTest
{
    [Fact]
    public void booleans_test_ok()
    {
        const string code = "true; false;";
        var expected = new StringToken[]
        {
            new("true", "boolean"),
            new(";", "punctuation"),
            new("false", "boolean"),
            new(";", "punctuation"),
        };
        TestHelper.TestCase(LanguageGrammar.CLike, code, expected);
    }
    
    [Fact]
    public void class_name_test_ok()
    {
        const string code = @"class Foo
interface bar
extends Foo
implements bar
trait Foo
instanceof \bar
new \Foo
catch (bar)";
        var expected = new Token[]
        {
            new StringToken("class "),
            new StreamToken(new StringToken[]
            {
                new("Foo")
            }, "class-name"),
            new StringToken("\ninterface "),
            new StreamToken(new StringToken[]
            {
                new("bar")
            }, "class-name"),
            new StringToken("\nextends "),
            new StreamToken(new StringToken[]
            {
                new("Foo")
            }, "class-name"),
            new StringToken("\nimplements "),
            new StreamToken(new StringToken[]
            {
                new("bar")
            }, "class-name"),
            new StringToken("\ntrait "),
            new StreamToken(new StringToken[]
            {
                new("Foo")
            }, "class-name"),
            new StringToken("instanceof", "keyword"),
            new StreamToken(new StringToken[]
            {
                new("\\", "punctuation"),
                new("bar")
            }, "class-name"),
            new StringToken("new", "keyword"),
            new StreamToken(new StringToken[]
            {
                new("\\", "punctuation"),
                new("Foo")
            }, "class-name"),
            new StringToken("catch", "keyword"),
            new StringToken("(", "punctuation"),
            new StreamToken(new StringToken[]
            {
                new("bar")
            }, "class-name"),
            new StringToken(")", "punctuation"),
        };
        TestHelper.TestCase(LanguageGrammar.CLike, code, expected);
    }
}