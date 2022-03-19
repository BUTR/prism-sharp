using Xunit;

namespace PrismSharp.Core.Tests.Languages;

// From https://github.com/PrismJS/prism/blob/master/tests/languages/regex/

public class RegExpTest
{
    [Fact]
    public void anchor_test_ok()
    {
        const string code = @"^
$
\A
\G
\Z
\z
\b
\B";
        var expected = new StringToken[]
        {
            new("^", "anchor"),
            new("$", "anchor"),
            new("\\A", "anchor"),
            new("\\G", "anchor"),
            new("\\Z", "anchor"),
            new("\\z", "anchor"),
            new("\\b", "anchor"),
            new("\\B", "anchor"),
        };
        TestHelper.RunTestCase(LanguageGrammars.RegExp, code, expected);
    }

    [Fact]
    public void backreference_test_ok()
    {
        const string code = @"\1 \2 \3 \4 \5 \6 \7 \8 \9
\k<name>";
        var expected = new Token[]
        {
            new StringToken("\\1", "backreference"),
            new StringToken("\\2", "backreference"),
            new StringToken("\\3", "backreference"),
            new StringToken("\\4", "backreference"),
            new StringToken("\\5", "backreference"),
            new StringToken("\\6", "backreference"),
            new StringToken("\\7", "backreference"),
            new StringToken("\\8", "backreference"),
            new StringToken("\\9", "backreference"),
            new StreamToken(new Token[]
            {
                new StringToken("\\k<"),
                new StringToken("name", "group-name"),
                new StringToken(">"),
            }, "backreference"),
        };
        TestHelper.RunTestCase(LanguageGrammars.RegExp, code, expected);
    }
}
