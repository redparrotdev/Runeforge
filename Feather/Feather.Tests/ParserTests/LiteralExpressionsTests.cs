using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class LiteralExpressionsTests
{
    [Fact]
    public void FeatherParser_ParsingStringLiteralExpression_ReturnsValidStringExpression()
    {
        var input = "\"Hello, World!\"";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LiteralExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.StringExpressing>(result);
        Assert.Equal("Hello, World!", ((FeatherExpression.StringExpressing)result).Value);
    }

    [Fact]
    public void FeatherParser_ParsingFloatNumberLiteral_ReturnsValidNumberExpression()
    {
        var input = "3.14";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LiteralExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.NumberExpressing>(result);
        Assert.Equal(3.14f, ((FeatherExpression.NumberExpressing)result).Value);
    }

    [Fact]
    public void FeatherParser_ParsingIntegerNumberLiteral_ReturnsValidNumberExpression()
    {
        var input = "42";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LiteralExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.NumberExpressing>(result);
        Assert.Equal(42f, ((FeatherExpression.NumberExpressing)result).Value);
    }

    [Fact]
    public void FeatherParser_ParsingTrueBooleanLiteral_ReturnsValidBooleanExpression()
    {
        var input = "true";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LiteralExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.BooleanExpression>(result);
        Assert.True(((FeatherExpression.BooleanExpression)result).Value);
    }

    [Fact]
    public void FeatherParser_ParsingFalseBooleanLiteral_ReturnsValidBooleanExpression()
    {
        var input = "false";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.LiteralExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.BooleanExpression>(result);
        Assert.False(((FeatherExpression.BooleanExpression)result).Value);
    }
}
