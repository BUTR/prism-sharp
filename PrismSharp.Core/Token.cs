namespace PrismSharp.Core;

public abstract class Token
{
    public string? Type { get; }
    public string[] Alias { get; }

    /// <summary>
    /// Copy of the full string this token was created from
    /// </summary>
    protected int Length { get; }

    protected Token(string? type, string[]? alias, string? matchedStr)
    {
        Type = type;
        Alias = alias ?? Array.Empty<string>();
        Length = matchedStr?.Length ?? 0;
    }

    public bool IsMatchedToken()
    {
        return Length > 0;
    }

    /// <summary>
    /// Return `Length` if current token is matched
    /// </summary>
    /// <returns></returns>
    public abstract int GetLength();

}

public class StringToken : Token
{
    public string Content { get; }

    public StringToken(string content,
        string? type = null,
        string[]? alias = null,
        string? matchedStr = null) : base(type, alias, matchedStr)
    {
        Content = content;
    }

    public override int GetLength()
    {
        return IsMatchedToken() ? Length : Content.Length;
    }
}

public class StreamToken : Token
{
    public Token[] Content { get; }

    public StreamToken(Token[] content,
        string? type = null,
        string[]? alias = null,
        string? matchedStr = null) : base(type, alias, matchedStr)
    {
        Content = content;
    }

    public override int GetLength()
    {
        return IsMatchedToken() ? Length : Content.Length;
    }
}
