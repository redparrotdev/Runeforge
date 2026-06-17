using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class DialogLineStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidDialogLine_WithCharacterName_ReturnsValidStatement()
    {
        var input = "<Main character> \"Something\"";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.DialogLineStatement.Parse(tokens);

        var lineStmt = Assert.IsType<FeatherStatement.DialogLineStatement>(result);
        Assert.Equal("Something", lineStmt.Text);
        Assert.IsType<FeatherExpression.CharacterNameExpression>(lineStmt.Character);
    }

    [Fact]
    public void FeatherParser_ParsingValidDialogLine_WithCharacterNameRef_ReturnsValidStatement()
    {
        var input = "<@char_name> \"Hello!\"";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.DialogLineStatement.Parse(tokens);

        var lineStms = Assert.IsType<FeatherStatement.DialogLineStatement>(result);
        Assert.Equal("Hello!", lineStms.Text);
        Assert.IsType<FeatherExpression.CharacterNameExpression>(lineStms.Character);
    }
}
