namespace PrismSharp.Core.Tests;

public class TestCaseFile
{
    public string Code { get; set; }
    public string Expected { get; set; }
    public string Description { get; set; }
    public string EOL { get; set; }
    public int? CodeLineStart { get; set; }
    public int? ExpectedLineStart { get; set; }
    public int? DescriptionLineStart { get; set; }

    public TestCaseFile(string code, string expected, string description)
    {
        Code = code;
        Expected = expected;
        Description = description;

        EOL = "\n"; // or \r\n

        CodeLineStart = null;
        ExpectedLineStart = null;
        DescriptionLineStart = null;
    }
}
