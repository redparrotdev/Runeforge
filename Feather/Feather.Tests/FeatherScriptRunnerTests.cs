using Feather.Core;
using Feather.Core.Structure;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Feather.Tests;

public sealed class FeatherScriptRunnerTests
{
    private static readonly FieldInfo TopLevelStatementsFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_topLevelStatements", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo LabelsMapFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_labelsMap", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo CurrentLabelFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_currentLabel", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo ScriptVariablesFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_scriptVariables", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo WaitingForChoiceFieldInfo = typeof(FeatherScriptRunner)
        .GetField("_waitingForChoiceSelection", BindingFlags.NonPublic | BindingFlags.Instance)!;

    [Fact]
    public void FeatherScriptRunner_Initialize_InitalizesCollectionsProperly()
    {
        var input = """
        START #main
        var a = 1
        var b = 2

        #main
        {
            <Hero> "Hello there"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(TopLevelStatementsFieldInfo);
        Assert.NotNull(LabelsMapFieldInfo);
        var topLevelStatements = (List<FeatherStatement>)TopLevelStatementsFieldInfo.GetValue(runner)!;
        var labelsMap = (Dictionary<string, FeatherStatement.LabelDeclarationStatement>)LabelsMapFieldInfo.GetValue(runner)!;
        Assert.NotNull(topLevelStatements);
        Assert.NotNull(labelsMap);
        Assert.NotEmpty(topLevelStatements);
        Assert.NotEmpty(labelsMap);
        Assert.True(topLevelStatements.Count == 2);
        Assert.True(labelsMap.Count == 1);
    }

    [Fact]
    public void FeatherScriptRunner_Initialize_JumpsToFirstLabel_WhenStartLabelIsProvided()
    {
        var input = """
        START #main

        #not_main
        {}

        #main
        {}
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(CurrentLabelFieldInfo);
        var currentLabel = (string)CurrentLabelFieldInfo.GetValue(runner)!;
        Assert.Equal("main", currentLabel);
    }

    [Fact]
    public void FeatherScriptRunner_Initialize_JumpsToFirstLabel_WhenStartLabelIsNotProvided()
    {
        var input = """
        #main
        {}
        #not_main
        {}
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(CurrentLabelFieldInfo);
        var currentLabel = (string)CurrentLabelFieldInfo.GetValue(runner)!;
        Assert.Equal("main", currentLabel);
    }

    [Fact]
    public void FeatherScriptRunner_Initialize_CreatesVariables()
    {
        var input = """
        var a = 1
        var b = "test"
        var c = true
        var d = null

        #main{}
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;

        var numVar = Assert.IsType<StrongBox<float>>(scriptVariables["a"]);
        Assert.Equal(1f, numVar.Value);

        var strVar = Assert.IsType<StrongBox<string>>(scriptVariables["b"]);
        Assert.Equal("test", strVar.Value);

        var boolVar = Assert.IsType<StrongBox<bool>>(scriptVariables["c"]);
        Assert.True(boolVar.Value);

        Assert.Null(scriptVariables["d"]);
    }

    [Fact]
    public void FeatherScriptRunner_ReturnsProperDialogLines_WhenEvaluatingScript()
    {
        var input = """
        #main
        {
            <Narator> "Hello there!"
            <Hero> "Hi!"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.True(runner.NextDialogLine());
        Assert.NotNull(runner.CurrentLine);
        Assert.Equal("Narator", runner.CurrentLine.CharacterName);
        Assert.Equal("Hello there!", runner.CurrentLine.Text);

        Assert.True(runner.NextDialogLine());
        Assert.NotNull(runner.CurrentLine);
        Assert.Equal("Hero", runner.CurrentLine.CharacterName);
        Assert.Equal("Hi!", runner.CurrentLine.Text);
    }

    [Fact]
    public void FeatherScriptRunner_NextDialogLine_ReturnsFalse_WhenNoMoreDialogsAvailable()
    {
        var input = """
        #main
        {
            <Narator> "Hello there!"
            <Hero> "Hi!"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();
        runner.NextDialogLine();
        Assert.False(runner.NextDialogLine());
    }

    [Fact]
    public void FeatherScriptRunner_NextDialogLine_ReturnsFalse_WhenNoMoreDialogLinesAwailable()
    {
        var input = """
        #main
        {
            <Hero> "Hi!"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        runner.NextDialogLine();
        Assert.False(runner.NextDialogLine());
    }

    [Fact]
    public void FeatherScriptRunner_SettingNewValueToVariable_ChangesItsValue()
    {
        var input = """
        var a = 1
        set a = 2
        #main{}
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);

        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.NotEmpty(scriptVariables);
        Assert.Equal(2f, (scriptVariables["a"] as StrongBox<float>)?.Value);
    }

    [Fact]
    public void FeatherScriptRunner_ReachingEndStatement_NotEvaluatesFurtherStatements()
    {
        var input = """
        var a = 1

        #main
        {
            <Hero> "Test"
            END
            set a = 2
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();
        
        Assert.False(runner.NextDialogLine());
        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.Equal(1f, (scriptVariables["a"] as StrongBox<float>)?.Value);
    }

    [Fact]
    public void FeatherScriptRunner_ReturnsValidChoices()
    {
        var input = """
        #main
        {
            <Narrator> "Hello there!"
            <Narrator> "How can I help you?"
            * "I looking for a job..."
            * "Just passing by"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();

        Assert.True(runner.NextDialogLine());
        var current = runner.CurrentLine;
        Assert.NotNull(current);
        Assert.NotEmpty(current.Choices);
        Assert.Equal(2, current.Choices.Count);
    }

    [Fact]
    public void FeatherScriptRunner_PassingChoice_RunsItsCode()
    {
        var input = """
        var a = 1

        #main
        {
            <Hero> "Let`s try something..."
            * "Option 1"
            {
                set a = 2
            }
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();

        var current = runner.CurrentLine;
        Assert.NotNull(current);
        Assert.NotNull(WaitingForChoiceFieldInfo);
        var waitingForChoice = (bool)WaitingForChoiceFieldInfo.GetValue(runner)!;
        Assert.True(waitingForChoice);

        var firstOption = current.Choices[0];
        Assert.False(runner.NextDialogLine(firstOption));

        Assert.NotNull(ScriptVariablesFieldInfo);
        var variables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.Equal(2f, (variables["a"] as StrongBox<float>)?.Value);
    }

    [Fact]
    public void FeatherScriptRunner_ReachingEndOnLabelBlock_NotExecutesAnyFurtherCode()
    {
        var input = """
        var a = 1
        
        #main
        {
            <Hero> "test"
            END
            set a = 2
            <Hero> "I should not say that"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();
        
        Assert.False(runner.NextDialogLine());
        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.Equal(1f, (scriptVariables["a"] as StrongBox<float>)?.Value);
    }

    [Fact]
    public void FeatherScriptRunner_ReachingEndOnChoiceBlock_NotExecutesAnyFurtherCode()
    {
        var input = """
        var a = 1

        #main
        {
            <Narrator> "Hello there!"
            * "Hi!"
            {
                END
                set a = 2
            }
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();
        var firstChoice = runner.CurrentLine!.Choices[0];

        Assert.False(runner.NextDialogLine(firstChoice));
        Assert.NotNull(ScriptVariablesFieldInfo);
        var scriptVariables = (Dictionary<string, object?>)ScriptVariablesFieldInfo.GetValue(runner)!;
        Assert.Equal(1f, (scriptVariables["a"] as StrongBox<float>)?.Value);
    }

    [Fact]
    public void FeatherScriptRunner_ReachingGoto_OnLabelBlock_MovesToTargetLabel()
    {
        var input = """
        #main
        {
            <Narrator> "Hello!"
            goto #second
        }

        #second
        {
            <Narrator> "There is another dialog line!"
        }
        """;

        var script = FeatherParser.ParseScript(input);
        var runner = new FeatherScriptRunner();
        runner.Initialize(script);
        runner.NextDialogLine();

        Assert.True(runner.NextDialogLine());
        Assert.Equal("There is another dialog line!", runner.CurrentLine?.Text);
        Assert.NotNull(CurrentLabelFieldInfo);
        var label = (string)CurrentLabelFieldInfo.GetValue(runner)!;
        Assert.Equal("second", label);
    }
}