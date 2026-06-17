using Feather.Core;
using Feather.Core.Structure;
using Superpower;

namespace Feather.Tests.ParserTests;

public sealed class VarStatementTests
{
    [Fact]
    public void FeatherParser_ParsingValidVarStatement_WithLiteral_ReturnsValidStatement()
    {
        var input = "var x = 42";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.VarStatement.Parse(tokens);

        Assert.IsType<FeatherStatement.VariableDeclarationStatement>(result);
        var varDecl = (FeatherStatement.VariableDeclarationStatement)result;
        Assert.Equal("x", varDecl.Name);
        Assert.IsType<FeatherExpression.NumberExpressing>(varDecl.Initializer);
    }

    [Fact]
    public void FeatherParser_ParsingValidVarStatement_WithIdentifier_ReturnsValidStatement()
    {
        var input = "var y = x";

        var tokens = FeatherTokenizer.Instance.Tokenize(input);
        var result = FeatherParser.VarStatement.Parse(tokens);

        Assert.IsType<FeatherStatement.VariableDeclarationStatement>(result);
        var varDecl = (FeatherStatement.VariableDeclarationStatement)result;
        Assert.Equal("y", varDecl.Name);
        var identifier = Assert.IsType<FeatherExpression.IdentifierExpressing>(varDecl.Initializer);
        Assert.Equal("x", identifier.Name);
    }
}
