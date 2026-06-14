using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class BasicLiteralsTests
{
    [Fact]
    public void FeatherParser_ParsingIdentifier_ReturnsValidIdentifier()
    {
        var input = "my_variable";
        var tokens = FeatherTokenizer.Instance.Tokenize(input);

        var result = FeatherParser.Identifier.Parse(tokens);

        Assert.Equal(input, result);
        Assert.Equal(FeatherTokenType.Identifier, tokens.FirstOrDefault().Kind);
    }

    [Fact]
    public void FeatherParser_ParsingStringLiteral_ReturnsValidString()
    {
        var input = "\"Hello, World!\"";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.StringLiteral.Parse(tokens);

        Assert.Equal(input, result);
        Assert.Equal(FeatherTokenType.String, tokens.FirstOrDefault().Kind);
    }

    [Fact]
    public void FeatherParser_ParsingFloatNumberLiteral_ReturnsValidNumber()
    {
        var input = "3.14";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.NumberLiteral.Parse(tokens);

        Assert.Equal(3.14f, result);
        Assert.Equal(FeatherTokenType.Number, tokens.FirstOrDefault().Kind);
    }

    [Fact]
    public void FeatherParser_ParsingIntegerNumberLiteral_ReturnsValidNumber()
    {
        var input = "42";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.NumberLiteral.Parse(tokens);

        Assert.Equal(42, result);
        Assert.Equal(FeatherTokenType.Number, tokens.FirstOrDefault().Kind);
    }
}