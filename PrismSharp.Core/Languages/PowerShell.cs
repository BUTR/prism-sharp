using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-powershell.js

public class PowerShell : IGrammarDefinition
{
    public Grammar Define()
    {
        var powershell = new Grammar
        {
            ["comment"] = new GrammarToken[]
            {
                new(@"(^|[^`])<#[\s\S]*?#>", true),
                new(@"(^|[^`])#.*", true),
            },
            ["string"] = new GrammarToken[]
            {
                new(@"""(?:`[\s\S]|[^`""])*""", greedy: true),
                new(@"'(?:[^']|'')*'", greedy: true),
            },
            // Matches name spaces as well as casts, attribute decorators. Force starting with letter to avoid matching array indices
            // Supports two levels of nested brackets (e.g. `[OutputType([System.Collections.Generic.List[int]])]`)
            ["namespace"] = new GrammarToken[]
            {
                new(new Regex(@"\[[a-z](?:\[(?:\[[^\]]*\]|[^\[\]])*\]|[^\[\]])*\]",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase))
            },
            ["boolean"] = new GrammarToken[]
            {
                new(new Regex(@"\$(?:false|true)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            },
            ["variable"] = new GrammarToken[] { new(@"\$\w+\b") },
            // Cmdlets and aliases. Aliases should come last, otherwise "write" gets preferred over "write-host" for example
            // Get-Command | ?{ $_.ModuleName -match "Microsoft.PowerShell.(Util|Core|Management)" }
            // Get-Alias | ?{ $_.ReferencedCommand.Module.Name -match "Microsoft.PowerShell.(Util|Core|Management)" }
            ["function"] = new GrammarToken[]
            {
                new(new Regex(
                    @"\b(?:Add|Approve|Assert|Backup|Block|Checkpoint|Clear|Close|Compare|Complete|Compress|Confirm|Connect|Convert|ConvertFrom|ConvertTo|Copy|Debug|Deny|Disable|Disconnect|Dismount|Edit|Enable|Enter|Exit|Expand|Export|Find|ForEach|Format|Get|Grant|Group|Hide|Import|Initialize|Install|Invoke|Join|Limit|Lock|Measure|Merge|Move|New|Open|Optimize|Out|Ping|Pop|Protect|Publish|Push|Read|Receive|Redo|Register|Remove|Rename|Repair|Request|Reset|Resize|Resolve|Restart|Restore|Resume|Revoke|Save|Search|Select|Send|Set|Show|Skip|Sort|Split|Start|Step|Stop|Submit|Suspend|Switch|Sync|Tee|Test|Trace|Unblock|Undo|Uninstall|Unlock|Unprotect|Unpublish|Unregister|Update|Use|Wait|Watch|Where|Write)-[a-z]+\b",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase)),
                new(new Regex(
                    @"\b(?:ac|cat|chdir|clc|cli|clp|clv|compare|copy|cp|cpi|cpp|cvpa|dbp|del|diff|dir|ebp|echo|epal|epcsv|epsn|erase|fc|fl|ft|fw|gal|gbp|gc|gci|gcs|gdr|gi|gl|gm|gp|gps|group|gsv|gu|gv|gwmi|iex|ii|ipal|ipcsv|ipsn|irm|iwmi|iwr|kill|lp|ls|measure|mi|mount|move|mp|mv|nal|ndr|ni|nv|ogv|popd|ps|pushd|pwd|rbp|rd|rdr|ren|ri|rm|rmdir|rni|rnp|rp|rv|rvpa|rwmi|sal|saps|sasv|sbp|sc|select|set|shcm|si|sl|sleep|sls|sort|sp|spps|spsv|start|sv|swmi|tee|trcm|type|write)\b",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase)),
            },
            // per http://technet.microsoft.com/en-us/library/hh847744.aspx
            ["keyword"] = new GrammarToken[]
            {
                new(new Regex(
                    @"\b(?:Begin|Break|Catch|Class|Continue|Data|Define|Do|DynamicParam|Else|ElseIf|End|Exit|Filter|Finally|For|ForEach|From|Function|If|InlineScript|Parallel|Param|Process|Return|Sequence|Switch|Throw|Trap|Try|Until|Using|Var|While|Workflow)\b",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase))
            },
            ["operator"] = new GrammarToken[]
            {
                new(
                    new Regex(
                        @"(^|\W)(?:!|-(?:b?(?:and|x?or)|as|(?:Not)?(?:Contains|In|Like|Match)|eq|ge|gt|is(?:Not)?|Join|le|lt|ne|not|Replace|sh[lr])\b|-[-=]?|\+[+=]?|[*\/%]=?)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true)
            },
            ["punctuation"] = new GrammarToken[]
            {
                new(@"[|{}[\];(),.]")
            }
        };

        // Variable interpolation inside strings, and nested expressions
        powershell["string"][0].Inside = new Grammar
        {
            ["function"] = new GrammarToken[]
            {
                // Allow for one level of nesting
                new(@"(^|[^`])\$\((?:\$\([^\r\n()]*\)|(?!\$\()[^\r\n)])*\)",
                    true,
                    inside: powershell)
            },
            ["boolean"] = powershell["boolean"],
            ["variable"] = powershell["variable"]
        };

        return powershell;
    }
}
