using System.Text.RegularExpressions;
using Xunit;

namespace PrismSharp.Core.Tests;

public class UtilTest
{
    [Fact]
    public void MatchPattern_lookbehind_eq_True_Ok()
    {
        var regex = new Regex(@"see (chapter \d+(\.\d)*)", RegexOptions.IgnoreCase);
        var text = @"For more information, see Chapter 3.4.5.1";
        var match = Util.MatchPattern(regex, 0, text, true);
        Assert.NotNull(match);
        Assert.Equal(3, match.Groups.Length);
        Assert.Equal(".5.1", match.Groups[0]);
        Assert.Equal("Chapter 3.4.5.1", match.Groups[1]);
        Assert.Equal(".1", match.Groups[2]);
        Assert.Equal(37, match.Index);
    }


}
