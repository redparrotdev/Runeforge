using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class CallStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidCallStatement_ReturnsValidStatement()
    {
        var input = "call MyFunction 42 name";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.CallStatement.Parse(tokens);

        var callStmt = Assert.IsType<FeatherStatement.FunctionCallStatement>(result);
        Assert.Equal("MyFunction", callStmt.FunctionName);
        Assert.NotEmpty(callStmt.Arguments);

        var args = callStmt.Arguments.ToArray();
        var arg1 = args[0];
        var arg2 = args[1];

        Assert.IsType<FeatherExpression.NumberExpressing>(arg1);
        Assert.IsType<FeatherExpression.IdentifierExpressing>(arg2);
    }
}
