using System.Text.RegularExpressions;

namespace Prism.Core;

public static class Prism
{
    public static Token[] Tokenize(string text, Grammar grammar)
    {
        var tokenList = new LinkedList<Token>();
        
        var head = new LinkedListNode<Token>(null!);
        var tail = new LinkedListNode<Token>(null!);
        var originTextNode = new LinkedListNode<Token>(new StringToken(text, "origin-text"));

        // var head = originTextNode;
        tokenList.AddFirst(head);
        tokenList.AddLast(tail);
        tokenList.AddAfter(head, originTextNode);

        MatchGrammar(text, tokenList, grammar, head, 0);
        return tokenList.Where(t => t != null!).ToArray();
    }

    // private static Token[] ToArray(LinkedList<Token> list)
    // {
    //     var tokens = new List<Token>();
    //     var node = list.First?.Next;
    //     while (node != list.Last) 
    //     {
    //         tokens.Add(node.Value);
    //         node = node.Next;
    //     }
    //     return tokens.ToArray();
    // }

    private static void MatchGrammar(string text, LinkedList<Token> tokenList, Grammar grammar,
        LinkedListNode<Token> startNode, int startPos, RematchOptions? rematch = null)
    {
        foreach (var (token, patterns) in grammar.GrammarTokenMap)
        {

            for (var j = 0; j < patterns.Length; ++j)
            {
                if (rematch?.Cause == token + ',' + j) 
                    return;
                
                var patternObj = patterns[j];
                var inside = patternObj.Inside;
                var lookbehind = patternObj.Lookbehind;
                var greedy = patternObj.Greedy;
                var alias = patternObj.Alias;
                
                // if (greedy && !patternObj.Pattern.m) {
                //     // Without the global flag, lastIndex won't work
                //     var flags = patternObj.pattern.toString().match(/[imsuy]*$/)[0];
                //     patternObj.pattern = RegExp(patternObj.pattern.source, flags + 'g');
                // }
                
                var pattern = patternObj.Pattern;
                
                // iterate the token list and keep track of the current token/string position
                var currentNode = startNode.Next;
                // var pos = startPos;
                
                for (var pos = startPos; 
                     currentNode != tokenList.Last;
                     pos += currentNode.Value.Length, currentNode = currentNode.Next)
                {
                    if (currentNode is null)
                        break;
                    
                    if (pos >= rematch?.Reach)
                        break;

                    if (tokenList.Count > text.Length)
                        // Something went terribly wrong, ABORT, ABORT!
                        return;

                    var tokenVal = currentNode.Value;

                    if (tokenVal is not StringToken stringToken)
                        continue;

                    var str = stringToken.Content;

                    var removeCount = 1; // this is the to parameter of removeBetween
                    Util.MyMatch match;

                    if (greedy)
                    {
                        match = Util.MatchPattern(pattern, pos, text, lookbehind);
                        if (!match.Success || match.Index >= text.Length)
                            break;

                        var fromIdx = match.Index;
                        var to = match.Index + match.Groups[0].Length;
                        var p = pos;

                        // find the node that contains the match
                        p += currentNode.Value.Length;
                        while (fromIdx >= p)
                        {
                            currentNode = currentNode.Next;
                            // if (currentNode is null)
                            //     break;
                            p += currentNode.Value.Length;
                        }

                        // if (currentNode is null)
                        //     break;

                        // adjust pos (and p)
                        p -= currentNode.Value.Length;
                        pos = p;

                        // the current node is a Token, then the match starts inside another Token, which is invalid
                        if (currentNode.Value is not StringToken)
                            continue;

                        // find the last node which is affected by this match
                        for (
                            var k = currentNode;
                            k != tokenList.Last && (p < to || k?.Value is StringToken);
                            k = k.Next)
                        {
                            if (k is null)
                                break;

                            removeCount++;
                            p += k.Value.Length;
                        }

                        removeCount--;

                        // replace with the new match
                        str = text.Substring(pos, p - pos);
                        match.Index -= pos;
                    }
                    else
                    {
                        match = Util.MatchPattern(pattern, 0, str, lookbehind);
                        if (!match.Success)
                            continue;
                    }

                    var from = match.Index;
                    var matchStr = match.Groups[0];
                    var before = str.Substring(0, from);
                    var after = str.Substring(from + matchStr.Length);

                    var reach = pos + str.Length;
                    if (reach > rematch?.Reach)
                        rematch.Reach = reach;

                    var removeFrom = currentNode.Previous;
                    if (removeFrom is null)
                        continue;

                    if (!string.IsNullOrEmpty(before))
                    {
                        var newNode = new LinkedListNode<Token>(new StringToken(before));
                        tokenList.AddAfter(removeFrom, newNode);
                        removeFrom = newNode;
                        pos += before.Length;
                    }

                    RemoveRange(tokenList, removeFrom, removeCount);

                    Token wrapped = inside != null
                        ? new StreamToken(Tokenize(matchStr, inside), token, alias, matchStr)
                        : new StringToken(matchStr, token, alias, matchStr);

                    var wrappedNode = new LinkedListNode<Token>(wrapped);
                    tokenList.AddAfter(removeFrom, wrappedNode);
                    currentNode = wrappedNode;

                    if (!string.IsNullOrEmpty(after))
                    {
                        var afterNode = new LinkedListNode<Token>(new StringToken(after));
                        tokenList.AddAfter(currentNode, afterNode);
                    }
                    
                    if (removeCount > 1) {
                        // at least one Token object was removed, so we have to do some rematching
                        // this can only happen if the current pattern is greedy

                        var nestedRematch = new RematchOptions
                        {
                            Cause = token + ',' + j,
                            Reach = reach
                        };
                        MatchGrammar(text, tokenList, grammar, currentNode.Previous, pos, nestedRematch);

                        // the reach might have been extended because of the rematching
                        if (nestedRematch.Reach > rematch?.Reach) 
                            rematch.Reach = nestedRematch.Reach;
                    }
                    
                }
            }
            
            
        }
    }
    
    /// <summary>
    /// Removes `count` nodes after the given node. The given node will not be removed.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="node"></param>
    /// <param name="count"></param>
    private static void RemoveRange(LinkedList<Token> list, LinkedListNode<Token> node, int count) 
    {
        var next = node.Next;
        for (var i = 0; i < count && next != list.Last && next is not null; i++)
        {
            list.Remove(next);
            next = next.Next;
        }
    }
    
    private class RematchOptions
    {
        public string Cause { get; set; } = null!;
        public int Reach { get; set; }
    }
}