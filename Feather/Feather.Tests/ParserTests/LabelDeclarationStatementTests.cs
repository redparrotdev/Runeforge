using Feather.Core;
using Feather.Core.Structure;
using Superpower;
using Superpower.Parsers;

namespace Feather.Tests.ParserTests;

public sealed class LabelDeclarationStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidLabelBlock_ReturnsValidStatement()
    {
        var input = """
        <NPC> "Hello there, traveler!"
        call setImage "npc.png"
        set a = 1
        <Hero> "Hi, have ane quests for me?"
        <NPC> "Sure, I do have a qeust for you!"
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LabelBlockStatement.Many().Parse(tokens);

        Assert.NotEmpty(result);
        Assert.True(result.Length == 5);
    }

    [Fact]
    public void FeatherParser_ParsingValidLabelDeclarationStatement_ReturnsValidStatement()
    {
        var input = """
        #start
        {
            <NPC> "Hello there, traveler!"
            call setImage "npc.png"
            set a = 1
            <Hero> "Hi, have ane quests for me?"
            <NPC> "Sure, I do have a qeust for you!"
            * "Listen to NPC"
            {
                goto #quest_start
            }
            * "Ignore NPC"
            {
                call SetMood "angry"
            }
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LabelDeclaration.Parse(tokens);

        var labelDeclStmt = Assert.IsType<FeatherStatement.LabelDeclarationStatement>(result);
        Assert.NotEmpty(labelDeclStmt.Choices);
    }
}
