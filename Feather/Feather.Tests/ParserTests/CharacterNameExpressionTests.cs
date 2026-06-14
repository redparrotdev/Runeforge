using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class CharacterNameExpressionTests
{
    [Fact]
    public void FeatherParser_ParsingCharacter_WithoutReference_ReturnsValidCharacterNameExpression()
    {
        var characterNameExpression = "<Main character>";

        var tokens = FeatherTokenizer.Instance.Tokenize(characterNameExpression);
        var result = FeatherParser.CharacterNameExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.CharacterNameExpression>(result);
        Assert.IsType<FeatherExpression.StringExpressing>((result as FeatherExpression.CharacterNameExpression)?.Value);
        Assert.Equal("Main character", ((result as FeatherExpression.CharacterNameExpression)?.Value as FeatherExpression.StringExpressing)?.Value);
    }

    [Fact]
    public void FeatherParser_ParsingCharacter_WithReference_ReturnsValidCharacterNameExpression()
    {
        var characterNameExpression = "<@character_ref>";

        var tokens = FeatherTokenizer.Instance.Tokenize(characterNameExpression);
        var result = FeatherParser.CharacterNameExpression.Parse(tokens);

        Assert.IsType<FeatherExpression.CharacterNameExpression>(result);
        Assert.IsType<FeatherExpression.IdentifierExpressing>((result as FeatherExpression.CharacterNameExpression)?.Value);
        Assert.Equal("character_ref", ((result as FeatherExpression.CharacterNameExpression)?.Value as FeatherExpression.IdentifierExpressing)?.Name);
    }
}
