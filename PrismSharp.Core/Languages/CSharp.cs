namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-csharp.js

public class CSharp : IGrammarDefinition
{
    public Grammar Define()
    {
        var extendCLikeGrammar = new CLike().Define();

        // TODO

        return extendCLikeGrammar;
    }

}
