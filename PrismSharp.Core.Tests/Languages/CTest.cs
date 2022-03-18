using PrismSharp.Core.Languages;
using Xunit;

namespace PrismSharp.Core.Tests.Languages;

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
        TestHelper.RunTestCase(LanguageGrammar.C, code, expected);
    }
}
