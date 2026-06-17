using Feather.Core.Structure;
using Superpower;
using Superpower.Parsers;

namespace Feather.Core;

public static class FeatherParser
{
    // Basics
    public static readonly TokenListParser<FeatherTokenType, string> Identifier 
        = Token
            .EqualTo(FeatherTokenType.Identifier)
            .Select(t => t.ToStringValue());

    public static readonly TokenListParser<FeatherTokenType, string> StringLiteral
        = Token
            .EqualTo(FeatherTokenType.String)
            .Select(t => t.ToStringValue());

    public static readonly TokenListParser<FeatherTokenType, float> NumberLiteral
        = Token
            .EqualTo(FeatherTokenType.Number)
            .Select(t => 
                float.Parse(t.ToStringValue(), System.Globalization.CultureInfo.InvariantCulture));

    // Expression
    public static readonly TokenListParser<FeatherTokenType, FeatherExpression> LiteralExpression
        = StringLiteral
            .Select(s => (FeatherExpression)new FeatherExpression.StringExpressing(s.Trim('"')))
            .Or(NumberLiteral
                .Select(n => (FeatherExpression)new FeatherExpression.NumberExpressing(n)))
            .Or(Token
                .EqualTo(FeatherTokenType.Null)
                .Select(_ => (FeatherExpression)new FeatherExpression.NullExpression()))
            .Or(Token
                .EqualTo(FeatherTokenType.True)
                .Select(_ => (FeatherExpression)new FeatherExpression.BooleanExpression(true)))
            .Or(Token
                .EqualTo(FeatherTokenType.False)
                .Select(_ => (FeatherExpression)new FeatherExpression.BooleanExpression(false)));

    public static readonly TokenListParser<FeatherTokenType, FeatherExpression> IdentifierExpression
        = Identifier
            .Select(id => (FeatherExpression)new FeatherExpression.IdentifierExpressing(id));

    public static readonly TokenListParser<FeatherTokenType, FeatherExpression> LabelIdentifier
        = Token
            .EqualTo(FeatherTokenType.Hash)
            .Then(_ => Identifier)
            .Select(label => (FeatherExpression)new FeatherExpression.LabelIdentifierExpression(label));

    public static readonly TokenListParser<FeatherTokenType, FeatherExpression> Expression
        = LiteralExpression
            .Or(IdentifierExpression);

    public static readonly TokenListParser<FeatherTokenType, FeatherExpression> CharacterNameExpression
       = Token
           .EqualTo(FeatherTokenType.Less)
           .Then(_ => Token
               .EqualTo(FeatherTokenType.AtSign)
               .Then(_ => Identifier
                   .Select(refName => (FeatherExpression)new FeatherExpression.IdentifierExpressing(refName)))
               .Or(Identifier
                    .Many()
                    .Select(nameParts =>
                    {
                        var fullName = string.Join(" ", nameParts);
                        return (FeatherExpression)new FeatherExpression.StringExpressing(fullName);
                    })))
           .Then(finalExpr => Token
               .EqualTo(FeatherTokenType.Greater)
               .Select(_ => (FeatherExpression)new FeatherExpression.CharacterNameExpression(finalExpr)));

    // Statements
    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> StartStatement
        = Token
            .EqualTo(FeatherTokenType.Start)
            .Then(_ => LabelIdentifier!)
            .Select(label => (FeatherStatement)new FeatherStatement.StartStatement((FeatherExpression.LabelIdentifierExpression)label));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> EndStatement
        = Token
            .EqualTo(FeatherTokenType.End)
            .Select(_ => (FeatherStatement)new FeatherStatement.EndStatement());

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> VarStatement
        = Token
            .EqualTo(FeatherTokenType.Var)
            .Then(_ => Identifier)
            .Then(name => Token
                .EqualTo(FeatherTokenType.Assign)
                .Then(_ => Expression)
                .Select(expr => (FeatherStatement)new FeatherStatement.VariableDeclarationStatement(name, expr)));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> GotoStatement
        = Token
            .EqualTo(FeatherTokenType.Goto)
            .Then(_ => LabelIdentifier)
            .Select(label => (FeatherStatement)new FeatherStatement.GotoStatement((FeatherExpression.LabelIdentifierExpression)label));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> ExpressionStatement
        = Expression
            .Select(expr => (FeatherStatement)new FeatherStatement.ExpressionStatement(expr));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> SetStatement
        = Token
            .EqualTo(FeatherTokenType.Set)
            .Then(_ => IdentifierExpression)
            .Then(name => Token
                .EqualTo(FeatherTokenType.Assign)
                .Then(_ => Expression)
                .Select(expr => (FeatherStatement)new FeatherStatement.SetVariableStatement((FeatherExpression.IdentifierExpressing)name, expr)));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> CallStatement
        = Token
            .EqualTo(FeatherTokenType.Call)
            .Then(_ => Identifier)
            .Then(functionName => Expression
                .Many()
                .Select(arguments => (FeatherStatement)new FeatherStatement.FunctionCallStatement(functionName, arguments)));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> DialogLineStatement
        = CharacterNameExpression
            .Then(character => StringLiteral
                .Select(text => (FeatherStatement)new FeatherStatement.DialogLineStatement((FeatherExpression.CharacterNameExpression)character, text.Trim('"'))));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> ChoiceBlockStatement
        = SetStatement
            .Or(CallStatement)
            .Or(GotoStatement)
            .Or(EndStatement);

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> ChoiceBlock
        = Token
            .EqualTo(FeatherTokenType.OpenBrace)
            .Then(_ => ChoiceBlockStatement.Many())
            .Then(statements => Token.EqualTo(FeatherTokenType.CloseBrace)
                .Select(__ => (FeatherStatement)new FeatherStatement.BlockStatement(statements)));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> ChoiceStatement
        = Token.EqualTo(FeatherTokenType.Star)
            .Then(_ => StringLiteral)
            .Then(text => ChoiceBlock!.OptionalOrDefault()
                .Select(block => (FeatherStatement)new FeatherStatement.ChoiceStatement(
                    text
                    , null
                    , (FeatherStatement.BlockStatement?)block)));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> LabelBlockStatement
        = ChoiceBlockStatement
            .Or(DialogLineStatement);

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> LabelDeclaration
        = LabelIdentifier
            .Then(label => Token
                .EqualTo(FeatherTokenType.OpenBrace)
                .Then(_ => LabelBlockStatement.Many())
                .Then(statements => 
                    ChoiceStatement
                    .Many()
                    .Then(choices =>
                        Token.EqualTo(FeatherTokenType.CloseBrace)
                        .Select(__ => (FeatherStatement)new FeatherStatement.LabelDeclarationStatement(
                            ((FeatherExpression.LabelIdentifierExpression)label).Label
                            , new FeatherStatement.BlockStatement(statements)
                            , choices.Cast<FeatherStatement.ChoiceStatement>())))));

    public static readonly TokenListParser<FeatherTokenType, FeatherStatement> TopLevelStatement
        = VarStatement
            .Or(CallStatement)
            .Or(SetStatement)
            .Or(LabelDeclaration);

    public static readonly TokenListParser<FeatherTokenType, FeatherScript> ScriptParser
        = StartStatement!
            .OptionalOrDefault()
            .Then(start => TopLevelStatement
                .AtLeastOnce()
                .Select(statements =>
                {
                    // Ensure at least one label declaration exists
                    if (!statements.Any(s => s is FeatherStatement.LabelDeclarationStatement))
                        throw new ParseException("At least one label declaration is required");
                    return new FeatherScript((FeatherStatement.StartStatement?)start, statements);
                }));
}
