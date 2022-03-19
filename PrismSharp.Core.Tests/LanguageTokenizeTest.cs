using System.Threading.Tasks;
using Xunit;

namespace PrismSharp.Core.Tests;

public class LanguageTokenizeTest
{
    [Theory]
    [InlineData("boolean_feature")]
    [InlineData("class-name_feature")]
    [InlineData("comment_feature")]
    [InlineData("function_feature")]
    [InlineData("keyword_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("string_feature")]
    [InlineData("issue1340")]
    public async Task test_CLike_features_ok(string testCase)
    {
        var testFile = $"./testcases/clike/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.CLike, testFile);
    }

    [Theory]
    [InlineData("char_feature")]
    [InlineData("class-name_feature")]
    [InlineData("comment_feature")]
    [InlineData("constant_feature")]
    [InlineData("function_feature")]
    [InlineData("keyword_feature")]
    [InlineData("macro_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("string_feature")]
    public async Task test_C_features_ok(string testCase)
    {
        var testFile = $"./testcases/c/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.C, testFile);
    }

    [Theory]
    [InlineData("boolean_feature")]
    [InlineData("class-method_feature")]
    [InlineData("constant_feature")]
    [InlineData("function-variable_feature")]
    [InlineData("function_feature")]
    [InlineData("getter-setter_feature")]
    [InlineData("hashbang_feature")]
    [InlineData("keyword_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("private_fields_feature")]
    [InlineData("property_feature")]
    [InlineData("regex_feature")] // TODO: test failed
    [InlineData("spread_and_keyword_feature")]
    [InlineData("supposed-classes_feature")]
    [InlineData("supposed-function_feature")]
    [InlineData("template-string_feature")]
    [InlineData("try-catch_feature")]
    [InlineData("variable_feature")]
    public async Task test_JavaScript_features_ok(string testCase)
    {
        var testFile = $"./testcases/javascript/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.JavaScript, testFile);
    }

    [Theory]
    [InlineData("cdata_feature")]
    [InlineData("comment_feature")]
    [InlineData("doctype_feature")]
    [InlineData("entity_feature")]
    [InlineData("prolog_feature")]
    [InlineData("tag_attribute_feature")]
    [InlineData("tag_feature")]
    [InlineData("issue585")]
    [InlineData("issue888")]
    public async Task test_Markup_features_ok(string testCase)
    {
        var testFile = $"./testcases/markup/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.Markup, testFile);
    }

    [Theory]
    [InlineData("anchor_feature")]
    [InlineData("backreference_feature")]
    [InlineData("char-class_feature")]
    [InlineData("char-set_feature")]
    [InlineData("escape_feature")]
    [InlineData("group_feature")]
    [InlineData("quantifier_feature")]
    [InlineData("range_feature")]
    public async Task test_RegExp_features_ok(string testCase)
    {
        var testFile = $"./testcases/regex/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.RegExp, testFile);
    }

    [Theory]
    [InlineData("boolean_feature")]
    [InlineData("comment_feature")]
    [InlineData("function_feature")]
    [InlineData("identifier_feature")]
    [InlineData("keyword_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("string_feature")]
    [InlineData("variable_feature")]
    [InlineData("issue3140")]
    public async Task test_Sql_features_ok(string testCase)
    {
        var testFile = $"./testcases/sql/{testCase}.test";
        await TestHelper.RunTestCaseFromFileAsync(LanguageGrammars.Sql, testFile);
    }
}
