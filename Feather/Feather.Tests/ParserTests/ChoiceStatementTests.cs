using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class ChoiceStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidChoiceStatement_WithBlock_ReturnsValidStatement()
    {
        var input = """
        * "Open shop"
        {
            call Set "shop" true
            set gold = 100
            goto #next
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.ChoiceStatement.Parse(tokens);

        var choiceStmt = Assert.IsType<FeatherStatement.ChoiceStatement>(result);
        Assert.Null(choiceStmt.Condition);
        Assert.IsType<FeatherStatement.BlockStatement>(choiceStmt.Block);
    }

    [Fact]
    public void FeatherParser_ParsingValidChoiceStatement_WithoutBlock_ReturnsValidStatement()
    {
        var input = """
        * "Open shop"
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.ChoiceStatement.Parse(tokens);

        var choiceStmt = Assert.IsType<FeatherStatement.ChoiceStatement>(result);
        Assert.Null(choiceStmt.Condition);
        Assert.Null(choiceStmt.Block);
    }

    [Fact]
    public void FeatherParser_ParsingInvalidChoiceBlock_ThrowsParseException()
    {
        var input = """
        {
            var a = 10
            goto #next
        }
        """;

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        Assert.Throws<ParseException>(() => FeatherParser.ChoiceBlock.Parse(tokens));
    }
}
