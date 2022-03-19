using Xunit;

namespace PrismSharp.Core.Tests.Languages;

public class SqlTest
{
    [Fact]
    public void boolean_test_ok()
    {
        const string code = @"TRUE
FALSE
NULL";
        var expected = new StringToken[]
        {
            new("TRUE", "boolean"),
            new("FALSE", "boolean"),
            new("NULL", "boolean"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Sql, code, expected);
    }

    [Fact]
    public void comment_test_ok()
    {
        const string code = @"/**/
/* foo
bar */
--
-- foo
//
// foo
#
# foo";
        var expected = new StringToken[]
        {
            new("/**/", "comment"),
            new("/* foo\nbar */", "comment"),
            new("--", "comment"),
            new("-- foo", "comment"),
            new("//", "comment"),
            new("// foo", "comment"),
            new("#", "comment"),
            new("# foo", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Sql, code, expected);
    }

    [Fact]
    public void number_test_ok()
    {
        const string code = @"42
0.154
0xBadFace";
        var expected = new StringToken[]
        {
            new("42","number"),
            new("0.154","number"),
            new("0xBadFace","number"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Sql, code, expected);
    }
}
