using PrismSharp.Core;
using Xunit;

namespace PrismSharp.Highlighting.HTML.Tests;

public class HtmlHighlighterTest
{
    [Fact]
    public void Highlight_markup_entity_ok()
    {
        var htmlHighlighter = new HtmlHighlighter();
        const string code = @"
&amp;
&thetasym;
&#65;
&#x41;
&#x26f5;";
        const string expected = @"
<span class=""token entity named-entity"">&amp;amp;</span>
<span class=""token entity named-entity"">&amp;thetasym;</span>
<span class=""token entity"">&amp;#65;</span>
<span class=""token entity"">&amp;#x41;</span>
<span class=""token entity"">&amp;#x26f5;</span>";

        var html = htmlHighlighter.Highlight(code, LanguageGrammars.Markup, "html");
        Assert.Equal(expected, html);
    }
}
