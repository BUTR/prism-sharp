using PrismSharp.Core.Languages;
using Xunit;

namespace PrismSharp.Core.Tests.Languages;

// From https://github.com/PrismJS/prism/blob/master/tests/languages/clike/

public class CLikeTest
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
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
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
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void comment_test_ok()
    {
        const string code = @"// foobar
/**/
/* foo
bar */";
        var expected = new StringToken[]
        {
            new("// foobar", "comment"),
            new("/**/", "comment"),
            new("/* foo\nbar */", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void comment_test_from_prismjs_issue1340_ok()
    {
        const string code = @"/*
//
*/";
        var expected = new StringToken[]
        {
            new("/*\n//\n*/", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void function_test_ok()
    {
        const string code = @"foo()
foo_bar()
f42()";
        var expected = new StringToken[]
        {
            new("foo", "function"),
            new("(", "punctuation"),
            new(")", "punctuation"),
            new("foo_bar", "function"),
            new("(", "punctuation"),
            new(")", "punctuation"),
            new("f42", "function"),
            new("(", "punctuation"),
            new(")", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void keyword_test_ok()
    {
        const string code = @"if; else; while; do; for;
return; in; instanceof; function; new;
try; throw; catch; finally; null;
break; continue;";
        var expected = new StringToken[]
        {
            new("if", "keyword"), new(";", "punctuation"),
            new("else", "keyword"), new(";", "punctuation"),
            new("while", "keyword"), new(";", "punctuation"),
            new("do", "keyword"), new(";", "punctuation"),
            new("for", "keyword"), new(";", "punctuation"),
            new("return", "keyword"), new(";", "punctuation"),
            new("in", "keyword"), new(";", "punctuation"),
            new("instanceof", "keyword"), new(";", "punctuation"),
            new("function", "keyword"), new(";", "punctuation"),
            new("new", "keyword"), new(";", "punctuation"),
            new("try", "keyword"), new(";", "punctuation"),
            new("throw", "keyword"), new(";", "punctuation"),
            new("catch", "keyword"), new(";", "punctuation"),
            new("finally", "keyword"), new(";", "punctuation"),
            new("null", "keyword"), new(";", "punctuation"),
            new("break", "keyword"), new(";", "punctuation"),
            new("continue", "keyword"), new(";", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void number_test_ok()
    {
        const string code = @"42
3.14159
4e10
2.1e-10
0.4e+2
0xbabe
0xBABE";
        var expected = new StringToken[]
        {
            new("42", "number"),
            new("3.14159", "number"),
            new("4e10", "number"),
            new("2.1e-10", "number"),
            new("0.4e+2", "number"),
            new("0xbabe", "number"),
            new("0xBABE", "number"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void operator_test_ok()
    {
        const string code = @"
- + -- ++
< <= > >=
= == ===
! != !==
& && | ||
? * / ~ ^ %";
        var expected = new StringToken[]
        {
            new("-", "operator"), new("+", "operator"), new("--", "operator"), new("++", "operator"),
            new("<", "operator"), new("<=", "operator"), new(">", "operator"), new(">=", "operator"),
            new("=", "operator"), new("==", "operator"), new("===", "operator"),
            new("!", "operator"), new("!=", "operator"), new("!==", "operator"),
            new("&", "operator"), new("&&", "operator"), new("|", "operator"), new("||", "operator"),
            new("?", "operator"), new("*", "operator"), new("/", "operator"), new("~", "operator"),
            new("^", "operator"), new("%", "operator"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }

    [Fact]
    public void string_test_ok()
    {
        const string code = @"""""
''
""f\""oo""
'b\'ar'
""foo\
bar""
'foo\
bar'
""foo /* comment */ bar""
'foo // bar'
'foo // bar' //comment";
        var expected = new StringToken[]
        {
            new("\"\"", "string"),
            new("''", "string"),
            new("\"f\\\"oo\"", "string"),
            new("'b\\'ar'", "string"),
            new("\"foo\\\nbar\"", "string"),
            new("'foo\\\nbar'", "string"),
            new("\"foo /* comment */ bar\"", "string"),
            new("'foo // bar'", "string"),
            new("'foo // bar'", "string"),
            new("//comment", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammar.CLike, code, expected);
    }
}
