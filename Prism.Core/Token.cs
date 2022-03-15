namespace Prism.Core;

public abstract class Token
{
    public string? Type { get; private set; }
    public string[] Alias { get; private set; }
    
    /// <summary>
    /// Copy of the full string this token was created from
    /// </summary>
    public int Length { get; private set; } = 0;

    public Token(string? type, string[]? alias, string? matchedStr)
    {
        Type = type;
        Alias = alias ?? Array.Empty<string>();
        Length = string.IsNullOrEmpty(matchedStr) ? 0 : matchedStr.Length;
    }

    public abstract int GetContentLength();
}

public class StringToken : Token
{
    public string Content { get; private set; }
    
    public StringToken(string content, string? type = null, string[]? alias = null, string? matchedStr = null) : base(type, alias, matchedStr)
    {
        Content = content;
    }

    public override int GetContentLength()
    {
        return Content.Length;
    }
}

public class StreamToken : Token
{
    public Token[] Content { get; private set; }

    public StreamToken(Token[] content, string? type = null, string[]? alias = null, string? matchedStr = null) : base(type, alias, matchedStr)
    {
        Content = content;
    }

    public override int GetContentLength()
    {
        return Content.Length;
    }
}