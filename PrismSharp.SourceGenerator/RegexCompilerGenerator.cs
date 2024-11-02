using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace PrismSharp.SourceGenerator;

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

[Generator]
public class RegexCompilerGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.outputpath", out var path))
            return;

        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.msbuildprojectfullpath", out var csPath))
            return;

        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.targetframework", out var targetFramework))
            return;

        var fullPath = Path.Combine(Path.GetDirectoryName(csPath)!, path);

        var languageDefinitions = LanguageDeclarationGenerator.GetFlattedLanguageDefinitions().ToArray();
        var languageDefinitionsHash = SHA256Hash(languageDefinitions.SelectMany(x => SHA256Hash(Encoding.UTF8.GetBytes(x.Value))).ToArray());
        var languageDefinitionsHashBase64 = Convert.ToBase64String(languageDefinitionsHash);

        var dllPath = Path.Combine(fullPath, "PrismSharp.RegEx.dll");
        var pdbPath = Path.Combine(fullPath, "PrismSharp.RegEx.pdb");

#pragma warning disable RS1035
        if (File.Exists(dllPath) && File.Exists(pdbPath))
        {
            try
            {
                using var stream = File.OpenRead(dllPath);
                using var reader = new PEReader(stream);

                var metadata = reader.GetMetadataReader();
                var attributes = GetAssemblyMetadataAttributes(metadata);
                foreach (var attribute in attributes)
                {
                    var attributeReader = metadata.GetBlobReader(attribute.Value);
                    attributeReader.ReadByte();
                    attributeReader.ReadByte();
                    var key = attributeReader.ReadSerializedString();
                    if (key != "Hash") continue;
                    var value = attributeReader.ReadSerializedString() ?? string.Empty;
                    if (value == languageDefinitionsHashBase64) return;
                }
            }
            catch (Exception) { /* ignore */ }
        }
#pragma warning restore RS1035

        if (targetFramework.StartsWith("net") && int.TryParse(targetFramework.Substring(3, 1), out var version))
        {
            if (version >= 8)
            {
                CompileNET80(languageDefinitions, languageDefinitionsHashBase64, dllPath, pdbPath);
            }
            else if (version <= 4)
            {
                CompileNET45(languageDefinitions, fullPath);
            }
            else
            {
                CompileNET45(languageDefinitions, fullPath);
                //CompileNETStandard20(languageDefinitions, languageDefinitionsHashBase64, dllPath, pdbPath);
            }
        }
        else
        {
            CompileNET45(languageDefinitions, fullPath);
            //CompileNETStandard20(languageDefinitions, languageDefinitionsHashBase64, dllPath, pdbPath);
        }
    }

    private static byte[] SHA256Hash(byte[] bytes)
    {
        using var sha = SHA512.Create();
        return sha.ComputeHash(bytes);
    }

    private static CustomAttribute[] GetAssemblyMetadataAttributes(MetadataReader metadata) => metadata.CustomAttributes
        .Select(metadata.GetCustomAttribute)
        .Where(x =>
        {
            var ctor = metadata.GetMemberReference((MemberReferenceHandle)x.Constructor);
            var attrType = metadata.GetTypeReference((TypeReferenceHandle)ctor.Parent);
            var name = metadata.GetString(attrType.Name);
            return name == "AssemblyMetadataAttribute";
        }).ToArray();

    private static Type? ByName(string name) => AppDomain.CurrentDomain.GetAssemblies()
        .Select(assembly => assembly.GetType(name))
        .OfType<Type>()
        .FirstOrDefault();

    private static void CompileNET80(KeyValuePair<string, string>[] languageDefinitions, string languageDefinitionsHashBase64, string dllPath, string pdbPath)
    {
        var list = GenerateLanguageRegularExpressions(languageDefinitions, languageDefinitionsHashBase64).ToArray();

        var compilation = CSharpCompilation.Create(
            "PrismSharp.RegEx",
            list.Select(x => CSharpSyntaxTree.ParseText(x.Value, path: $"{x.Key}.cs", encoding: Encoding.UTF8)),
            Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        //new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithPlatform(Platform.AnyCpu).WithOptimizationLevel(OptimizationLevel.Release));

        //var sourceGenDriver = CSharpGeneratorDriver.Create(new System.Text.RegularExpressions.Generator.RegexGenerator());
        var type = ByName("System.Text.RegularExpressions.Generator.RegexGenerator")!;
        var sourceGenDriver = CSharpGeneratorDriver.Create((IIncrementalGenerator)Activator.CreateInstance(type)!);
        sourceGenDriver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();
        var emitResult = outputCompilation.Emit(dllStream, pdbStream);
        if (!emitResult.Success)
        {
            // emitResult.Diagnostics
        }

        dllStream.Seek(0, SeekOrigin.Begin);
        using (var fileStream = new FileStream(dllPath, FileMode.Create, FileAccess.Write))
        {
            dllStream.CopyTo(fileStream);
        }

        pdbStream.Seek(0, SeekOrigin.Begin);
        using (var fileStream = new FileStream(pdbPath, FileMode.Create, FileAccess.Write))
        {
            pdbStream.CopyTo(fileStream);
        }
    }

#pragma warning disable RS1035
    private static void CompileNET45(KeyValuePair<string, string>[] languageDefinitions, string outputDirectory)
    {
        // The generated Regex seem to work on any platform, but the generator itself
        // only works on .NET Framework, so we launch a separate process to generate the Regex
        var asm = typeof(RegexCompilerGenerator).Assembly.GetManifestResourceStream("PrismSharp.RegExCompiler.exe")!;
        var tempPath = $"{Path.GetTempFileName()}.exe";
        var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
        try
        {
            asm.CopyTo(fs);
            fs.Dispose();

            var languages = languageDefinitions.Select(languageDefinition =>
            {
                var language = languageDefinition.Key;
                var source = languageDefinition.Value;

                var flatted = Flatted.Parse(source)!;
                var grammar = GrammarTokenParser.GetGrammar(flatted);
                var regularExpressions = GrammarTokenParser.GetAllRegularExpressions(grammar);

                return new LanguageEntry
                {
                    Language = language,
                    Patterns = regularExpressions.Select(kv => new PatternEntry
                    {
                        Name = kv.Key,
                        Pattern = kv.Value,
                    }).ToList()
                };
            }).ToList();
            var root = new Root { Languages = languages };

            var tempPathXml = $"{Path.GetTempFileName()}.xml";
            var fsXml = new FileStream(tempPathXml, FileMode.Create, FileAccess.Write);
            try
            {
                new XmlSerializer(typeof(Root)).Serialize(fsXml, root);
                fsXml.Dispose();

                // File API is forbidden, but you're fine to execute a process? Really?
                var psi = new ProcessStartInfo
                {
                    FileName = tempPath,
                    Arguments = tempPathXml,
                    WorkingDirectory = outputDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var p = Process.Start(psi);
                p?.WaitForExit();
            }
            finally
            {
                fsXml.Dispose();
                File.Delete(tempPathXml);
            }
        }
        finally
        {
            fs.Dispose();
            File.Delete(tempPath);
        }
    }
#pragma warning restore RS1035

    private static void CompileNETStandard20(KeyValuePair<string, string>[] languageDefinitions, string languageDefinitionsHashBase64, string dllPath, string pdbPath)
    {
        var list = GenerateLanguageRegularExpressionsSimple(languageDefinitions, languageDefinitionsHashBase64).ToArray();

        var compilation = CSharpCompilation.Create(
            "PrismSharp.RegEx",
            list.Select(x => CSharpSyntaxTree.ParseText(x.Value, path: $"{x.Key}.cs", encoding: Encoding.UTF8)),
            NetStandard20.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        //new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithPlatform(Platform.AnyCpu).WithOptimizationLevel(OptimizationLevel.Release));

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();
        var emitResult = compilation.Emit(dllStream, pdbStream);
        if (!emitResult.Success)
        {
            // emitResult.Diagnostics
        }

        dllStream.Seek(0, SeekOrigin.Begin);
        using (var fileStream = new FileStream(dllPath, FileMode.Create, FileAccess.Write))
        {
            dllStream.CopyTo(fileStream);
        }

        pdbStream.Seek(0, SeekOrigin.Begin);
        using (var fileStream = new FileStream(pdbPath, FileMode.Create, FileAccess.Write))
        {
            pdbStream.CopyTo(fileStream);
        }
    }

    private static IEnumerable<KeyValuePair<string, string>> GenerateLanguageRegularExpressionsSimple(KeyValuePair<string, string>[] languageDefinitions, string languageDefinitionsHashBase64)
    {
        yield return new("AssemblyMetadata", $$"""
                                             [assembly: System.Reflection.AssemblyMetadata("Hash", "{{languageDefinitionsHashBase64}}")]
                                             """);

        foreach (var languageDefinition in languageDefinitions)
        {
            var language = languageDefinition.Key;
            var source = languageDefinition.Value;

            var flatted = Flatted.Parse(source)!;
            var grammar = GrammarTokenParser.GetGrammar(flatted);
            var regularExpressions = GrammarTokenParser.GetAllRegularExpressions(grammar);

            var sb = new StringBuilder();
            sb.AppendLine($$"""
                            using System;
                            using System.Text.RegularExpressions;

                            namespace PrismSharp.RegEx.{{language}};
                            """);
            foreach (var kv in regularExpressions)
            {
                sb.AppendLine($$"""
                                public class {{kv.Key}} : Regex
                                {
                                    public {{kv.Key}}() : base(@"{{kv.Value.Replace("\"", "\"\"")}}") { }
                                }
                                """);
            }

            yield return new(language, sb.ToString());
        }
    }

    private static IEnumerable<KeyValuePair<string, string>> GenerateLanguageRegularExpressions(KeyValuePair<string, string>[] languageDefinitions, string languageDefinitionsHashBase64)
    {
        yield return new("AssemblyMetadata", $$"""
                                             [assembly: System.Reflection.AssemblyMetadata("Hash", "{{languageDefinitionsHashBase64}}")]
                                             """);

        yield return new("RegexWrapper", """
                                         using System;
                                         using System.Text.RegularExpressions;

                                         namespace PrismSharp.RegEx;

                                         public class RegexWrapper : Regex
                                         {
                                             [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "pattern")]
                                             private static extern ref string GetPattern(Regex c);

                                             [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "roptions")]
                                             private static extern ref RegexOptions GetROptions(Regex c);

                                             [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "internalMatchTimeout")]
                                             private static extern ref TimeSpan GetInternalMatchTimeout(Regex c);

                                             [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "factory")]
                                             private static extern ref RegexRunnerFactory GetFactory(Regex c);

                                             [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "capsize")]
                                             private static extern ref int GetCapsize(Regex c);

                                             protected void Initialize(Regex instance)
                                             {
                                                 pattern = GetPattern(instance);
                                                 roptions = GetROptions(instance);
                                                 internalMatchTimeout = GetInternalMatchTimeout(instance);
                                                 factory = GetFactory(instance);
                                                 capsize = GetCapsize(instance);
                                             }
                                         }
                                         """);

        foreach (var languageDefinition in languageDefinitions)
        {
            var language = languageDefinition.Key;
            var source = languageDefinition.Value;

            var flatted = Flatted.Parse(source)!;
            var grammar = GrammarTokenParser.GetGrammar(flatted);
            var regularExpressions = GrammarTokenParser.GetAllRegularExpressions(grammar);

            var sb = new StringBuilder();
            sb.AppendLine($$"""
                            using System;
                            using System.Text.RegularExpressions;

                            namespace PrismSharp.RegEx.{{language}};

                            internal static partial class GeneratedRegex
                            {
                            """);
            foreach (var kv in regularExpressions)
            {
                sb.AppendLine($$"""
                                    [GeneratedRegex(@"{{kv.Value.Replace("\"", "\"\"")}}")]
                                    public static partial Regex {{kv.Key}}();

                                """);
            }

            sb.AppendLine("}");

            foreach (var kv in regularExpressions)
            {
                sb.AppendLine($$"""
                                public class {{kv.Key}} : RegexWrapper
                                {
                                    public {{kv.Key}}()
                                    {
                                        Initialize(GeneratedRegex.{{kv.Key}}());
                                    }
                                }
                                """);
            }

            yield return new(language, sb.ToString());
        }
    }
}
