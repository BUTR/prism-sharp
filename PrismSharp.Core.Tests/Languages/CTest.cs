using PrismSharp.Core.Languages;
using Xunit;

namespace PrismSharp.Core.Tests.Languages;

// From https://github.com/PrismJS/prism/blob/master/tests/languages/c/

public class CTest
{
    [Fact]
    public void char_test_ok()
    {
        const string code = @"'a'
'\n'
'\13'
'🍌'
'ab'

u'a'
u'¢'
u'猫'
U'猫'
L'猫'

'\1\2\3\4'
'\xFF'
u'\U0001f34c'";
        var expected = new StringToken[]
        {
            new("'a'", "char"),
            new("'\\n'", "char"),
            new("'\\13'", "char"),
            new("'🍌'", "char"),
            new("'ab'", "char"),

            new("\n\nu"), new("'a'", "char"),
            new("\nu"), new("'¢'", "char"),
            new("\nu"), new("'猫'", "char"),
            new("\nU"), new("'猫'", "char"),
            new("\nL"), new("'猫'", "char"),

            new("'\\1\\2\\3\\4'", "char"),
            new("'\\xFF'", "char"),
            new("\nu"), new("'\\U0001f34c'", "char"),
        };
        TestHelper.RunTestCase(LanguageGrammars.C, code, expected);
    }

    [Fact]
    public void comment_test_ok()
    {
        const string code = @"//
// comment
// the comment \
   continues!

/**/
/*
 * comment
 */

/* open-ended comment";
        var expected = new StringToken[]
        {
            new("//", "comment"),
            new("// comment", "comment"),
            new("// the comment \\\n   continues!", "comment"),
            new("/**/", "comment"),
            new("/*\n * comment\n */", "comment"),
            new("/* open-ended comment", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammars.C, code, expected);
    }

    [Fact]
    public void constant_test_ok()
    {
        const string code = @"__FILE__
__LINE__
__DATE__
__TIME__
__TIMESTAMP__
__func__
EOF
NULL
SEEK_CUR
SEEK_END
SEEK_SET
stdin
stdout
stderr";
        var expected = new StringToken[]
        {
            new("__FILE__", "constant"),
            new("__LINE__", "constant"),
            new("__DATE__", "constant"),
            new("__TIME__", "constant"),
            new("__TIMESTAMP__", "constant"),
            new("__func__", "constant"),
            new("EOF", "constant"),
            new("NULL", "constant"),
            new("SEEK_CUR", "constant"),
            new("SEEK_END", "constant"),
            new("SEEK_SET", "constant"),
            new("stdin", "constant"),
            new("stdout", "constant"),
            new("stderr", "constant"),
        };
        TestHelper.RunTestCase(LanguageGrammars.C, code, expected);
    }

    [Fact]
    public void function_test_ok()
    {
        const string code = @"foo(void);
bar
(1, 2);";
        var expected = new StringToken[]
        {
            new("foo", "function"),
            new("(", "punctuation"),
            new("void", "keyword"),
            new(")", "punctuation"),
            new(";", "punctuation"),
            new("bar", "function"),
            new("(", "punctuation"),
            new("1", "number"),
            new(",", "punctuation"),
            new("2", "number"),
            new(")", "punctuation"),
            new(";", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammars.C, code, expected);
    }

    [Fact]
    public void class_name_test_ok()
    {
        const string code = @"struct foo;
enum bar;

struct foo var;
struct __attribute__ ((aligned (8))) S { short f[3]; };

// by name
uint32_t foo;
static dtrace_helptrace_t *bar;";
        var expected = new StringToken[]
        {
            new("struct", "keyword"),
            new("foo", "class-name"),
            new(";", "punctuation"),
            new("enum", "keyword"),
            new("bar", "class-name"),
            new(";", "punctuation"),

            new("struct", "keyword"),
            new("foo", "class-name"),
            new(" var"),
            new(";", "punctuation"),
            new("struct", "keyword"),
            new("__attribute__", "keyword"),
            new("(", "punctuation"),
            new("(", "punctuation"),
            new("aligned", "function"),
            new("(", "punctuation"),
            new("8", "number"),
            new(")", "punctuation"),
            new(")", "punctuation"),
            new(")", "punctuation"),
            new("S", "class-name"),
            new("{", "punctuation"),
            new("short", "keyword"),
            new(" f"),
            new("[", "punctuation"),
            new("3", "number"),
            new("]", "punctuation"),
            new(";", "punctuation"),
            new("}", "punctuation"),
            new(";", "punctuation"),

            new("// by name", "comment"),
            new("uint32_t", "class-name"),
            new(" foo"),
            new(";", "punctuation"),
            new("static", "keyword"),
            new("dtrace_helptrace_t", "class-name"),
            new("*", "operator"),
            new("bar"),
            new(";", "punctuation"),
        };
        TestHelper.RunTestCase(LanguageGrammars.C, code, expected);
    }
}
