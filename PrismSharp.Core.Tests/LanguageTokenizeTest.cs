using System.IO;
using System.Linq;
using Xunit;

namespace PrismSharp.Core.Tests;

// TODO: create the class by source-generator
public class LanguageTokenizeTest
{
    [Fact]
    public void test_CLike_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/clike/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.CLike, testFile));
    }

    [Fact]
    public void test_C_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/c/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.C, testFile));
    }

    [Fact]
    public void test_Cpp_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/cpp/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Cpp, testFile));
    }

    [Theory]
    [InlineData("csharp")]
    // [InlineData("csharp!+xml-doc")]
    public void test_CSharp_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.CSharp, testFile));
    }

    [Theory]
    [InlineData("comment_feature")]
    [InlineData("page-directive_feature")]
    public void test_AspNet_features_ok(string testCase)
    {
        var testFile = $"./testcases/aspnet/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.AspNet, testFile);
    }

    [Theory]
    [InlineData("directive_feature")]
    public void test_CSharp_AspNet_features_ok(string testCase)
    {
        var testFile = $"./testcases/csharp+aspnet/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.AspNet, testFile);
    }


    [Fact]
    public void test_CSHtml_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/cshtml/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.CSHtml, testFile));

    }

    [Fact]
    public void test_JavaScript_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/javascript/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.JavaScript, testFile));
    }

    [Fact]
    public void test_Markup_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/markup/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Markup, testFile));
    }

    [Theory]
    [InlineData("markup!+css")]
    [InlineData("markup!+css+javascript")]
    [InlineData("markup!+javascript")]
    [InlineData("markup+javascript+csharp+aspnet")]
    public void test_Markup_inline_Others_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Markup, testFile));
    }

    [Fact]
    public void test_RegExp_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/regex/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.RegExp, testFile));
    }

    [Fact]
    public void test_Sql_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/sql/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Sql, testFile));
    }

    [Fact]
    public void test_Json_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/json/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Json, testFile));
    }

    [Fact]
    public void test_PowerShell_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/powershell/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.PowerShell, testFile));
    }

    [Fact]
    public void test_Yaml_features_ok()
    {
        var testFiles = Directory.GetFiles("./testcases/yaml/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Yaml, testFile));
    }

    [Theory]
    [InlineData("css")]
    public void test_Css_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Css, testFile));
    }

    [Theory]
    [InlineData("lua")]
    public void test_Lua_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Lua, testFile));
    }

    [Theory]
    [InlineData("bash")]
    public void test_Bash_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Bash, testFile));
    }

    [Theory]
    [InlineData("batch")]
    public void test_Batch_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Batch, testFile));
    }

    [Theory]
    [InlineData("go")]
    public void test_Go_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Go, testFile));
    }

    [Theory]
    [InlineData("rust")]
    public void test_Rust_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Rust, testFile));
    }

    [Theory]
    [InlineData("python")]
    public void test_Python_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Python, testFile));
    }

    [Theory]
    [InlineData("java")]
    public void test_Java_all_features_ok(string testCase)
    {
        var testFiles = Directory.GetFiles($"./testcases/{testCase}/", "*.test")
            .Where(testFile => !testFile.EndsWith(".html.test")).ToArray();
        Assert.NotEmpty(testFiles);
        Assert.All(testFiles, testFile => TestHelper.RunTestCaseFromFile(LanguageGrammars.Java, testFile));
    }
}
