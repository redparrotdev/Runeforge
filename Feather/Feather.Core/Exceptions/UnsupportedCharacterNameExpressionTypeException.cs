namespace Feather.Core.Exceptions;

public sealed class UnsupportedCharacterNameExpressionTypeException : FeatherException
{
    public Type ExpressionType { get; private set; }

    public UnsupportedCharacterNameExpressionTypeException(Type expressionType) : this(expressionType, null)
    {
    }

    public UnsupportedCharacterNameExpressionTypeException(Type expressionType, Exception? innerException)
        : base($"Unsupported character name expression type: {expressionType.FullName}", innerException)
    {
        ExpressionType = expressionType;
    }
}
