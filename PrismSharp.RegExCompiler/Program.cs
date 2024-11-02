using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace PrismSharp.RegExCompiler;

[XmlRoot("Root")]
public class Root
{
    [XmlElement("Languages")]
    public List<LanguageEntry> Languages { get; set; } = new();
}

public class LanguageEntry
{
    [XmlAttribute("Language")]
    public string Language { get; set; } = default!;

    [XmlElement("Patterns")]
    public List<PatternEntry> Patterns { get; set; } = new();
}

public class PatternEntry
{
    [XmlAttribute("Name")]
    public string Name { get; set; } = default!;

    [XmlElement("Pattern")]
    public string Pattern { get; set; } = default!;
}

public static class Program
{
    public static void Main(string[] args)
    {
        var path = args[0];
        var xml = File.ReadAllText(path);
        var data = (Root)new XmlSerializer(typeof(Root)).Deserialize(new StringReader(xml));

        Regex.CompileToAssembly(
            data.Languages.SelectMany(x => x.Patterns.Select(kv => new RegexCompilationInfo(kv.Pattern, RegexOptions.Compiled, kv.Name, $"PrismSharp.RegEx.{x.Language}", true))).ToArray(),
            new AssemblyName("PrismSharp.RegEx"));
    }
}
