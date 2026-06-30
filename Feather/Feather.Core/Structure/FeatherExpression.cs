namespace Feather.Core.Structure;

public abstract record FeatherExpression
{
    public record StringExpressing(string Value) : FeatherExpression;
    public record NumberExpressing(float Value) : FeatherExpression;
    public record BooleanExpression(bool Value) : FeatherExpression;
    public record NullExpression() : FeatherExpression;
    public record IdentifierExpressing(string Name) : FeatherExpression;
    public record LabelIdentifierExpression(string Label) : FeatherExpression;
    public record CharacterNameExpression(FeatherExpression Value) : FeatherExpression;
    public record BinaryExpression(FeatherExpression Left, BinaryOperatorType Operator, FeatherExpression Right) : FeatherExpression;
}
