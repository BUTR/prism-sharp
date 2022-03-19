using Xunit;

namespace PrismSharp.Core.Tests.Languages;

// From https://github.com/PrismJS/prism/blob/master/tests/languages/javascript/

public class JavaScriptTest
{
    [Fact]
    public void boolean_test_ok()
    {
        const string code = "true; false;";
        var expected = new StringToken[]
        {
            new("true", "boolean"),
            new(";", "punctuation"),
            new("false", "boolean"),
            new(";", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammars.JavaScript, code, expected);
    }

    [Fact]
    public void constant_test_ok()
    {
        const string code = @"var FOO;
const FOO_BAR;
const BAZ42;
gl.FLOAT_MAT2x4;";
        var expected = new StringToken[]
        {
            new("var", "keyword"),
            new("FOO", "constant"),
            new(";", "punctuation"),

            new("const", "keyword"),
            new("FOO_BAR", "constant"),
            new(";", "punctuation"),

            new("const", "keyword"),
            new("BAZ42", "constant"),
            new(";", "punctuation"),

            new("\ngl"),
            new(".", "punctuation"),
            new("FLOAT_MAT2x4", "constant"),
            new(";", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammars.JavaScript, code, expected);
    }
}
