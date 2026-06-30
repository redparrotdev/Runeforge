using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class BinaryExpressionsParserTests
{
    [Theory]
    [InlineData("(1)", typeof(FeatherExpression.NumberExpressing))]
    [InlineData("(\"Some string\")", typeof(FeatherExpression.StringExpressing))]
    [InlineData("(true)", typeof(FeatherExpression.BooleanExpression))]
    [InlineData("(null)", typeof(FeatherExpression.NullExpression))]
    [InlineData("(some_var)", typeof(FeatherExpression.IdentifierExpressing))]
    public void FeatherParser_ParsingParenthesizedExpression_ReturnsValidExpressionType(string input, Type expectedType)
    {

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.Parenthesized.Parse(tokens);

        Assert.IsType(expectedType, result);
    }

    [Theory]
    [InlineData("1", typeof(FeatherExpression.NumberExpressing))]
    [InlineData("\"Some string\"", typeof(FeatherExpression.StringExpressing))]
    [InlineData("true", typeof(FeatherExpression.BooleanExpression))]
    [InlineData("null", typeof(FeatherExpression.NullExpression))]
    [InlineData("some_var", typeof(FeatherExpression.IdentifierExpressing))]
    [InlineData("(1)", typeof(FeatherExpression.NumberExpressing))]
    [InlineData("(\"Some string\")", typeof(FeatherExpression.StringExpressing))]
    [InlineData("(true)", typeof(FeatherExpression.BooleanExpression))]
    [InlineData("(null)", typeof(FeatherExpression.NullExpression))]
    [InlineData("(some_var)", typeof(FeatherExpression.IdentifierExpressing))]
    public void FeatherParser_ParsingPrimaryExpression_ReturnsValidExpressionType(string input, Type expectedType)
    {
        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.PrimaryExpression.Parse(tokens);

        Assert.IsType(expectedType, result);
    }

    [Theory]
    [InlineData("*", BinaryOperatorType.Multiply)]
    [InlineData("/", BinaryOperatorType.Divide)]
    public void FeatherParser_ParsingMultiplicativeOperator_ReturnsValidOperatorType(string input, BinaryOperatorType operatorType)
    {
        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.MultiplicativeOperator.Parse(tokens);
        Assert.Equal(operatorType, result);
    }

    [Theory]
    [InlineData("+", BinaryOperatorType.Plus)]
    [InlineData("-", BinaryOperatorType.Minus)]
    public void FeatherParser_ParsingAdditiveOperator_ReturnsValidOperatorType(string input, BinaryOperatorType operatorType)
    {
        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveOperator.Parse(tokens);
        Assert.Equal(operatorType, result);
    }

    [Fact]
    public void FeatherParser_ParsingSimpleBinaryExpression_ReturnsValidExpression()
    {
        var input = "1 + 1";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveExpression.Parse(tokens);

        Assert.NotNull(result);
        var binary = Assert.IsType<FeatherExpression.BinaryExpression>(result);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary.Left);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary.Right);
        Assert.Equal(BinaryOperatorType.Plus, binary.Operator);
    }

    [Fact]
    public void FeatherParser_ParsingMultiplePartsBinaryExpression_ReturnsValidExpression()
    {
        var input = "1 + 1 * 2";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveExpression.Parse(tokens);

        Assert.NotNull(result);
        var binary = Assert.IsType<FeatherExpression.BinaryExpression>(result);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary.Left);
        Assert.IsType<FeatherExpression.BinaryExpression>(binary.Right);
        Assert.Equal(BinaryOperatorType.Plus, binary.Operator);
    }

    [Fact]
    public void FeatherParser_ParsingBinaryExpressionWithParentheses_ReturnsValidExpression()
    {
        var input = "(1 + 1) * 2";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveExpression.Parse(tokens);

        Assert.NotNull(result);
        var binary = Assert.IsType<FeatherExpression.BinaryExpression>(result);
        Assert.IsType<FeatherExpression.BinaryExpression>(binary.Left);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary.Right);
        Assert.Equal(BinaryOperatorType.Multiply, binary.Operator);
    }

    [Fact]
    public void FeatherParser_ParsingComplexBinaryExpression_ReturnsValidExpression()
    {
        var input = "1 + 2 * (3 - 4) / 5";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveExpression.Parse(tokens);

        Assert.NotNull(result);
        // 1 + binary2
        var binary1 = Assert.IsType<FeatherExpression.BinaryExpression>(result);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary1.Left);
        var binary2 = Assert.IsType<FeatherExpression.BinaryExpression>(binary1.Right);
        Assert.Equal(BinaryOperatorType.Plus, binary1.Operator);

        // binary3 / 5
        var binary3 = Assert.IsType<FeatherExpression.BinaryExpression>(binary2.Left);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary2.Right);
        Assert.Equal(BinaryOperatorType.Divide, binary2.Operator);

        // 2 * binary4
        Assert.IsType<FeatherExpression.NumberExpressing>(binary3.Left);
        var binary4 = Assert.IsType<FeatherExpression.BinaryExpression>(binary3.Right);
        Assert.Equal(BinaryOperatorType.Multiply, binary3.Operator);

        Assert.IsType<FeatherExpression.NumberExpressing>(binary4.Left);
        Assert.IsType<FeatherExpression.NumberExpressing>(binary4.Right);
        Assert.Equal(BinaryOperatorType.Minus, binary4.Operator);
    }

    [Fact]
    public void FeatherParser_ParsingBinaryExpressionWithStrings_ReturnsValidExpression()
    {
        var input = "\"Hello \" + \"World!\"";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.AdditiveExpression.Parse(tokens);

        Assert.NotNull(result);
        var binary = Assert.IsType<FeatherExpression.BinaryExpression>(result);
        Assert.IsType<FeatherExpression.StringExpressing>(binary.Left);
        Assert.IsType<FeatherExpression.StringExpressing>(binary.Right);
    }
}
