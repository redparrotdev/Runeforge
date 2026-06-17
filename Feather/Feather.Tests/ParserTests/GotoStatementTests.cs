using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class GotoStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidGotoStatement_ReturnsValidStatement()
    {
        var input = "goto #label";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.GotoStatement.Parse(tokens);

        var stmt = Assert.IsType<FeatherStatement.GotoStatement>(result);
        var labelExpr = Assert.IsType<FeatherExpression.LabelIdentifierExpression>(stmt.Label);
        Assert.Equal("label", labelExpr.Label);
    }

    [Fact]
    public void FeatherParser_ParsingInvalidGotoStatement_ThrowsParseException()
    {
        var input = "goto label";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);

        Assert.Throws<ParseException>(() => FeatherParser.GotoStatement.Parse(tokens));
    }
}
