using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class LabelIdentifierExpressionTests
{
    [Fact]
    public void FeatherParser_ParsingLabelIdentifier_ReturnsValidLabelIdentifierExpression()
    {
        var input = "#start";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LabelIdentifier.Parse(tokens);

        Assert.IsType<FeatherExpression.LabelIdentifierExpression>(result);
        Assert.Equal("start", ((FeatherExpression.LabelIdentifierExpression)result).Label);
    }
}
