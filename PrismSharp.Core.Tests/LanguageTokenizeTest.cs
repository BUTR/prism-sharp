using Xunit;

namespace PrismSharp.Core.Tests;

// TODO: create the class by source-generator
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
    public void test_CLike_features_ok(string testCase)
    {
        var testFile = $"./testcases/clike/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.CLike, testFile);
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
    public void test_C_features_ok(string testCase)
    {
        var testFile = $"./testcases/c/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.C, testFile);
    }

    [Theory]
    [InlineData("attribute_feature")]
    [InlineData("char_feature")]
    [InlineData("class-name-declaration_feature")]
    [InlineData("class-name-variables-parameters_feature")]
    [InlineData("constructor-invocation_feature")]
    [InlineData("for_feature")]
    [InlineData("generic-constraint_feature")]
    [InlineData("generic_feature")]
    [InlineData("interpolation-string_feature")]
    [InlineData("keyword_feature")]
    [InlineData("named-parameter_feature")]
    [InlineData("namespace_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("preprocessor_feature")]
    [InlineData("punctuation_feature")]
    [InlineData("range_feature")]
    [InlineData("return-type_feature")]
    [InlineData("string_feature")]
    [InlineData("switch_feature")]
    [InlineData("type-expression_feature")]
    [InlineData("type-list_feature")]
    [InlineData("using-directive_feature")]
    [InlineData("issue1091")]
    [InlineData("issue1365")]
    [InlineData("issue1371")]
    [InlineData("issue2968")]
    [InlineData("issue806")]
    public void test_CSharp_features_ok(string testCase)
    {
        var testFile = $"./testcases/csharp/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.CSharp, testFile);
    }

    [Theory]
    [InlineData("comment_feature")]
    [InlineData("page-directive_feature")] // TODO: test failed
    public void test_AspNet_features_ok(string testCase)
    {
        var testFile = $"./testcases/aspnet/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.AspNet, testFile);
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
    [InlineData("regex_feature")]
    [InlineData("spread_and_keyword_feature")]
    [InlineData("supposed-classes_feature")]
    [InlineData("supposed-function_feature")]
    [InlineData("template-string_feature")]
    [InlineData("try-catch_feature")]
    [InlineData("variable_feature")]
    public void test_JavaScript_features_ok(string testCase)
    {
        var testFile = $"./testcases/javascript/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.JavaScript, testFile);
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
    public void test_Markup_features_ok(string testCase)
    {
        var testFile = $"./testcases/markup/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.Markup, testFile);
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
    public void test_RegExp_features_ok(string testCase)
    {
        var testFile = $"./testcases/regex/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.RegExp, testFile);
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
    public void test_Sql_features_ok(string testCase)
    {
        var testFile = $"./testcases/sql/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.Sql, testFile);
    }

    [Theory]
    [InlineData("boolean_feature")]
    [InlineData("comment_feature")]
    [InlineData("null_feature")]
    [InlineData("number_feature")]
    [InlineData("operator_feature")]
    [InlineData("property_feature")]
    [InlineData("punctuation_feature")]
    [InlineData("string_feature")]
    [InlineData("issue1852")]
    public void test_Json_features_ok(string testCase)
    {
        var testFile = $"./testcases/json/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.Json, testFile);
    }

    [Theory]
    [InlineData("boolean_feature")]
    [InlineData("comment_feature")]
    [InlineData("function_feature")]
    [InlineData("keyword_feature")]
    [InlineData("namespace_feature")]
    [InlineData("operator_feature")]
    [InlineData("string_feature")]
    [InlineData("variable_feature")]
    [InlineData("issue1407")]
    public void test_PowerShell_features_ok(string testCase)
    {
        var testFile = $"./testcases/powershell/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.PowerShell, testFile);
    }

    [Theory]
    [InlineData("anchor_and_alias_feature")]
    [InlineData("boolean_feature")]
    [InlineData("comment_feature")]
    [InlineData("datetime_feature")]
    [InlineData("directive_feature")]
    [InlineData("important_feature")]
    [InlineData("key_feature")]
    [InlineData("null_feature")]
    [InlineData("number_feature")]
    [InlineData("scalar_feature")]
    [InlineData("string_feature")]
    [InlineData("tag_feature")]
    public void test_Yaml_features_ok(string testCase)
    {
        var testFile = $"./testcases/yaml/{testCase}.test";
        TestHelper.RunTestCaseFromFile(LanguageGrammars.Yaml, testFile);
    }
}
