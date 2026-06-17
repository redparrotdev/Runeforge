using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class FeatherScriptTests
{
    [Fact]
    public void FeatherParser_ParsingValidFeatherScript_ReturnsValidScript()
    {
        var input = """
        START #main

        var a = 10
        var b = "test"

        #main
        {
            call setImage "background.png"

            <Narrator> "Hello there, traveler!"
            <Narrator> "What brought you here today?"
            * "I`m looking for adventures!"
            {
                call setImage "adventure.png"
                goto #quest
            }
            * "Just passing by..."
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.ScriptParser.Parse(tokens);

        var script = Assert.IsType<FeatherScript>(result);
        Assert.NotNull(script.StartLabel);
        Assert.NotEmpty(script.TopLevelStatements);
        Assert.True(script.TopLevelStatements.Count() == 3);
    }

    [Fact]
    public void FeatherParser_ParsingScriptWithIntermixedStatementsAndLabels_ReturnsValidScript()
    {
        var input = """
        START #main
        
        var a = 10
        
        #first
        {
            <NPC> "First label"
        }
        
        var b = "test"
        
        #main
        {
            <Narrator> "Main label"
            goto #first
        }
        
        call someFunction "arg"
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.ScriptParser.Parse(tokens);

        var script = Assert.IsType<FeatherScript>(result);
        Assert.NotNull(script.StartLabel);
        Assert.NotEmpty(script.TopLevelStatements);
        Assert.True(script.TopLevelStatements.Count() == 5);
        Assert.True(script.TopLevelStatements.Count(s => s is FeatherStatement.LabelDeclarationStatement) == 2);
        Assert.True(script.TopLevelStatements.Count(s => s is FeatherStatement.VariableDeclarationStatement) == 2);
        Assert.True(script.TopLevelStatements.Count(s => s is FeatherStatement.FunctionCallStatement) == 1);
    }

    [Fact]
    public void FeatherParser_ParsingScriptWithoutLabels_ThrowsParseException()
    {
        var input = """
        START #main
        
        var a = 10
        var b = "test"
        call someFunction "arg"
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        
        Assert.Throws<ParseException>(() => FeatherParser.ScriptParser.Parse(tokens));
    }

    [Fact]
    public void FeatherParser_ParsinValidScriptWithoutStartStatement_GivesNullStartStatement()
    {
        var input = """
        var a = 1
        var b = "test"

        #main
        {
            <Narrator> "Hey, hear me out!"
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.ScriptParser.Parse(tokens);

        var script = Assert.IsType<FeatherScript>(result);
        Assert.Null(script.StartLabel);
    }

    [Fact]
    public void FeatherParser_ParsingScriptWithStartStatementNotAtTheStart_ThrowsparsinError()
    {
        var input = """
        var a = 1
        var b = 2

        START #main

        #main
        {
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);

        Assert.Throws<ParseException>(() => FeatherParser.ScriptParser.Parse(tokens));
    }
}
