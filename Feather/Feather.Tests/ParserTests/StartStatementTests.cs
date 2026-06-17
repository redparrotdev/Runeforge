using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class StartStatementTests
{
    [Fact]
    public void FeatherParser_ParsingStartStatement_ReturnsValidStatement()
    {
        var input = "START #begin";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.StartStatement.Parse(tokens);

        Assert.IsType<FeatherStatement.StartStatement>(result);
    }
}
