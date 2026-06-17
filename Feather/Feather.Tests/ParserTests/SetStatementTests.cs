using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class SetStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidSetStatement_ReturnsValidStatement()
    {
        var input = "set gold = 100";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.SetStatement.Parse(tokens);

        var setStmt = Assert.IsType<FeatherStatement.SetVariableStatement>(result);
        Assert.Equal("gold", setStmt.Variable);
        Assert.IsType<FeatherExpression.NumberExpressing>(setStmt.Value);
    }
}
