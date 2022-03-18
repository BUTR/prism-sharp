namespace PrismSharp.Core;

public interface IHighlighter
{
    /// <summary>
    /// Highlight `text` by `grammar`
    /// </summary>
    /// <param name="text"></param>
    /// <param name="grammar"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    string Highlight(string text, Grammar grammar, string language);
}
