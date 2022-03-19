using Xunit;

namespace PrismSharp.Core.Tests.Languages;

// From https://github.com/PrismJS/prism/blob/master/tests/languages/markup/

public class MarkupTest
{
    [Fact]
    public void cdata_test_ok()
    {
        const string code = @"<![CDATA[ foo bar baz ]]>
<![CDATA[
foo bar baz
]]>";
        var expected = new StringToken[]
        {
            new("<![CDATA[ foo bar baz ]]>", "cdata"),
            new("<![CDATA[\nfoo bar baz\n]]>", "cdata"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void comment_test_ok()
    {
        const string code = @"<!---->
<!-- foo bar -->
<!-- foo bar
baz -->";
        var expected = new StringToken[]
        {
            new("<!---->", "comment"),
            new("<!-- foo bar -->", "comment"),
            new("<!-- foo bar\nbaz -->", "comment"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void prolog_test_ok()
    {
        const string code = @"<?xml version=""1.0""?>
<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet href=""tei2html.xsl""
type=""text/xsl""?>";
        var expected = new StringToken[]
        {
            new("<?xml version=\"1.0\"?>", "prolog"),
            new("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "prolog"),
            new("<?xml-stylesheet href=\"tei2html.xsl\"\ntype=\"text/xsl\"?>", "prolog"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void entity_test_ok()
    {
        const string code = @"&amp;
&thetasym;
&#65;
&#x41;
&#x26f5;
#foo;";
        var expected = new StringToken[]
        {
            new("&amp;", "entity"),
            new("&thetasym;", "entity"),
            new("&#65;", "entity"),
            new("&#x41;", "entity"),
            new("&#x26f5;", "entity"),
            new("\n#foo;"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void doctype_test_ok()
    {
        const string code = @"<!DOCTYPE html>
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"">
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""
""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<!DOCTYPE greeting SYSTEM ""hello.dtd"">
<!DOCTYPE greeting [
	<!ELEMENT greeting (#PCDATA)>
]>
<!DOCTYPE greeting [
	<!ELEMENT greeting (#PCDATA)>
	<!ELEMENT subject (#PCDATA)>
	<!-- comment ]> -->
]>";
        var expected = new Token[]
        {
            new StreamToken(new StringToken[]
            {
                new("<!", "punctuation"),
                new("DOCTYPE", "doctype-tag"),
                new("html", "name"),
                new(">", "punctuation"),
            }, "doctype"),
            new StreamToken(new StringToken[]
            {
                new("<!", "punctuation"),
                new("DOCTYPE", "doctype-tag"),
                new("HTML", "name"),
                new("PUBLIC", "name"),
                new("\"-//W3C//DTD HTML 4.01//EN\"", "string"),
                new(">", "punctuation"),
            }, "doctype"),
            new StreamToken(new StringToken[]
            {
                new("<!", "punctuation"),
                new("DOCTYPE", "doctype-tag"),
                new("html", "name"),
                new("PUBLIC", "name"),
                new("\"-//W3C//DTD XHTML 1.0 Strict//EN\"", "string"),
                new("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"", "string"),
                new(">", "punctuation"),
            }, "doctype"),
            new StreamToken(new StringToken[]
            {
                new("<!", "punctuation"),
                new("DOCTYPE", "doctype-tag"),
                new("greeting", "name"),
                new("SYSTEM", "name"),
                new("\"hello.dtd\"", "string"),
                new(">", "punctuation"),
            }, "doctype"),
            new StreamToken(new Token[]
            {
                new StringToken("<!", "punctuation"),
                new StringToken("DOCTYPE", "doctype-tag"),
                new StringToken("greeting", "name"),
                new StringToken("[", "punctuation"),
                new StreamToken(new []
                {
                    new StreamToken(new Token[]
                    {
                        new StreamToken(new StringToken[]
                        {
                            new("<", "punctuation"),
                            new("!ELEMENT"),
                        }, "tag"),
                        new StreamToken(new StringToken[]
                        {
                            new("greeting"),
                        }, "attr-name"),
                        new StreamToken(new StringToken[]
                        {
                            new("(#PCDATA)"),
                        }, "attr-name"),
                        new StringToken(">", "punctuation")
                    }, "tag")
                }, "internal-subset"),
                new StringToken("]", "punctuation"),
                new StringToken(">", "punctuation"),
            }, "doctype"),
            new StreamToken(new Token[]
            {
                new StringToken("<!", "punctuation"),
                new StringToken("DOCTYPE", "doctype-tag"),
                new StringToken("greeting", "name"),
                new StringToken("[", "punctuation"),
                new StreamToken(new Token[]
                {
                    new StreamToken(new Token[]
                    {
                        new StreamToken(new StringToken[]
                        {
                            new("<", "punctuation"),
                            new("!ELEMENT"),
                        }, "tag"),
                        new StreamToken(new StringToken[]
                        {
                            new("greeting"),
                        }, "attr-name"),
                        new StreamToken(new StringToken[]
                        {
                            new("(#PCDATA)"),
                        }, "attr-name"),
                        new StringToken(">", "punctuation")
                    }, "tag"),
                    new StreamToken(new Token[]
                    {
                        new StreamToken(new StringToken[]
                        {
                            new("<", "punctuation"),
                            new("!ELEMENT"),
                        }, "tag"),
                        new StreamToken(new StringToken[]
                        {
                            new("subject"),
                        }, "attr-name"),
                        new StreamToken(new StringToken[]
                        {
                            new("(#PCDATA)"),
                        }, "attr-name"),
                        new StringToken(">", "punctuation")
                    }, "tag"),
                    new StringToken("<!-- comment ]> -->", "comment")
                }, "internal-subset"),
                new StringToken("]", "punctuation"),
                new StringToken(">", "punctuation"),
            }, "doctype"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void tag_test_ok()
    {
        const string code = @"<p></p>
<div>dummy</div>
<div
> </div
>
<foo:bar> </foo:bar>
<div";
        var expected = new Token[]
        {
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("p"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("</", "punctuation"),
                    new ("p"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StringToken("dummy"),
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("</", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("</", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("foo:", "namespace"),
                    new ("bar"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("</", "punctuation"),
                    new ("foo:", "namespace"),
                    new ("bar"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
            new StringToken("\n<div")
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void tag_attribute_test_ok()
    {
        const string code = @"<div class=""test"" foo bar=baz>
<div foo='bar'>
<div class=""foo
bar
baz"">
<div foo:bar=42>
<div foo = 42 bar = ""42"">
<div foo = ""=\""bar=baz/>";
        var expected = new Token[]
        {
            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new("class")
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new("=", "punctuation"),
                    new("\"", "punctuation"),
                    new("test"),
                    new("\"", "punctuation"),
                }, "attr-value"),
                new StreamToken(new StringToken[]
                {
                    new("foo")
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new("bar")
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new("=", "punctuation"),
                    new("baz"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new ("foo"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("'", "punctuation"),
                    new ("bar"),
                    new ("'", "punctuation"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new ("class"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("\"", "punctuation"),
                    new ("foo\nbar\nbaz"),
                    new ("\"", "punctuation"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new ("foo:", "namespace"),
                    new ("bar"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("42"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new ("foo"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new (" 42"),
                }, "attr-value"),
                new StreamToken(new StringToken[]
                {
                    new ("bar"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("\"", "punctuation"),
                    new ("42"),
                    new ("\"", "punctuation"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new StringToken[]
                {
                    new ("<", "punctuation"),
                    new ("div"),
                }, "tag"),
                new StreamToken(new StringToken[]
                {
                    new ("foo"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("\"", "punctuation"),
                    new ("=\\"),
                    new ("\"", "punctuation"),
                }, "attr-value"),
                new StreamToken(new StringToken[]
                {
                    new ("bar"),
                }, "attr-name"),
                new StreamToken(new StringToken[]
                {
                    new ("=", "punctuation"),
                    new ("baz/"),
                }, "attr-value"),
                new StringToken(">", "punctuation")
            }, "tag"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void unicode_characters_test_ok_from_prismjs_issue585()
    {
        const string code = @"<Läufer>foo</Läufer>
<tag läufer=""läufer"">
<läufer:tag>baz</läufer:tag>";
        var expected = new Token[]
        {
            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("<", "punctuation"),
                    new StringToken("Läufer"),
                }, "tag"),
                new StringToken(">", "punctuation"),
            }, "tag"),
            new StringToken("foo"),
            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("</", "punctuation"),
                    new StringToken("Läufer"),
                }, "tag"),
                new StringToken(">", "punctuation"),
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("<", "punctuation"),
                    new StringToken("tag"),
                }, "tag"),
                new StreamToken(new Token[]
                {
                    new StringToken("läufer"),
                }, "attr-name"),
                new StreamToken(new Token[]
                {
                    new StringToken("=", "punctuation"),
                    new StringToken("\"", "punctuation"),
                    new StringToken("läufer"),
                    new StringToken("\"", "punctuation"),
                }, "attr-value"),
                new StringToken(">", "punctuation"),
            }, "tag"),

            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("<", "punctuation"),
                    new StringToken("läufer:", "namespace"),
                    new StringToken("tag"),
                }, "tag"),
                new StringToken(">", "punctuation"),
            }, "tag"),
            new StringToken("baz"),
            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("</", "punctuation"),
                    new StringToken("läufer:", "namespace"),
                    new StringToken("tag"),
                }, "tag"),
                new StringToken(">", "punctuation"),
            }, "tag"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }

    [Fact]
    public void tag_name_containing_dots_test_ok_from_prismjs_issue888()
    {
        const string code = @"<android.support.v7.widget.CardView>";
        var expected = new Token[]
        {
            new StreamToken(new Token[]
            {
                new StreamToken(new Token[]
                {
                    new StringToken("<", "punctuation"),
                    new StringToken("android.support.v7.widget.CardView"),
                }, "tag"),
                new StringToken(">", "punctuation")
            }, "tag"),
        };
        TestHelper.RunTestCase(LanguageGrammars.Markup, code, expected);
    }
}
