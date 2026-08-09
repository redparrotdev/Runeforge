using Feather.Core;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Feather.Tests;

public sealed class FeatherScriptRunner_ExpressionsTests
{
    private static readonly FieldInfo ScriptVariablesFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_scriptVariables", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [Theory]
    [InlineData("1 + 3", typeof(StrongBox<float>), 4f)]
    [InlineData("\"Hello\" + \"World\"", typeof(StrongBox<string>), "HelloWorld")]
    [InlineData("true + true", typeof(StrongBox<float>), 2f)]
    [InlineData("1 + true", typeof(StrongBox<float>), 2f)]
    [InlineData("6 / 2", typeof(StrongBox<float>), 3f)]
    [InlineData("\"Hello\" * 2", typeof(StrongBox<string>), "HelloHello")]
    [InlineData("\"HelloWorld\" - \"World\"", typeof(StrongBox<string>), "Hello")]
    // Complex expressions
    [InlineData("1 + 2 * 3", typeof(StrongBox<float>), 7f)]
    [InlineData("(1 + 2) * 3", typeof(StrongBox<float>), 9f)]
    [InlineData("1 + 2 * 3 / 4", typeof(StrongBox<float>), 2.5f)]
    public void FeatherScriptRunner_ExecutingVarStatementWithBinaryExpressions_ReturnsValidResult(
        string variableExpression
        , Type expectedVariableType
        , object? expectedValue)
    {
        var input = $@"
        var a = {variableExpression}

        #main{{}}
        ";

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.True(scriptVariables.TryGetValue("a", out var aVar));
        Assert.IsType(expectedVariableType, aVar);
        var value = aVar.GetType().GetField("Value")?.GetValue(aVar);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("1 + 3", typeof(StrongBox<float>), 4f)]
    [InlineData("\"Hello\" + \"World\"", typeof(StrongBox<string>), "HelloWorld")]
    [InlineData("true + true", typeof(StrongBox<float>), 2f)]
    [InlineData("1 + true", typeof(StrongBox<float>), 2f)]
    [InlineData("6 / 2", typeof(StrongBox<float>), 3f)]
    [InlineData("\"Hello\" * 2", typeof(StrongBox<string>), "HelloHello")]
    [InlineData("\"HelloWorld\" - \"World\"", typeof(StrongBox<string>), "Hello")]
    // Complex expressions
    [InlineData("1 + 2 * 3", typeof(StrongBox<float>), 7f)]
    [InlineData("(1 + 2) * 3", typeof(StrongBox<float>), 9f)]
    [InlineData("1 + 2 * 3 / 4", typeof(StrongBox<float>), 2.5f)]
    public void FeatherScriptRunner_ExecutingSetStatementWithBinaryExpressions_SetsValidResult(
        string expression
        , Type expectedType
        , object? expectedValue)
    {
        var input = $@"
        var a = 0
        set a = {expression}
        #main{{}}
        ";

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.True(scriptVariables.TryGetValue("a", out var aVar));
        Assert.IsType(expectedType, aVar);
        var value = aVar.GetType().GetField("Value")?.GetValue(aVar);
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void FeatherScriptRunner_ExecutingVarStatementWithBinaryExpressionContainingVariable_ReturnsValidResult()
    {
        var input = @"
        var a = 5
        var b = a + 3
        #main{}
        ";

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.True(scriptVariables.TryGetValue("b", out var bVar));
        Assert.IsType<StrongBox<float>>(bVar);
        var value = bVar.GetType().GetField("Value")?.GetValue(bVar);
        Assert.Equal(8f, value);
    }

    [Fact]
    public void FeatherScriptRunner_ExecutingSetStatementWithBinaryExpressionContainingVariable_SetsValidResult()
    {
        var input = @"
        var a = 5
        var b = 0
        set b = a + 3
        #main{}
        ";

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.True(scriptVariables.TryGetValue("b", out var bVar));
        Assert.IsType<StrongBox<float>>(bVar);
        var value = bVar.GetType().GetField("Value")?.GetValue(bVar);
        Assert.Equal(8f, value);
    }
}
