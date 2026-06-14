using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class IdentifierExpressionTests
{
    [Fact]
    public void FeatherParser_ParsingIdentifier_ReturnsValidIdentifierExpression()
    {
        var input = "myVariable";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.IdentifierExpression.Parse(tokens);
        Assert.IsType<FeatherExpression.IdentifierExpressing>(result);
        Assert.Equal(input, ((FeatherExpression.IdentifierExpressing)result).Name);
    }

    [Fact]
    public void FeatherParser_ParsingInvalidIdentifier_ThrowsParseException()
    {
        var input = "123InvalidIdentifier";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        Assert.Throws<ParseException>(() => FeatherParser.IdentifierExpression.Parse(tokens));
    }
}
