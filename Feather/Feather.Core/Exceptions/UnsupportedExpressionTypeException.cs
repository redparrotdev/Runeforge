namespace Feather.Core.Exceptions;

public sealed class UnsupportedExpressionTypeException : FeatherException
{
    public Type ExpressionType { get; private set; }

    public UnsupportedExpressionTypeException(Type expressionType) : this(expressionType, null)
    {
    }

    public UnsupportedExpressionTypeException(Type expressionType, Exception? innerException)
        : base($"Unsupported expression type: {expressionType.FullName}", innerException)
    {
        ExpressionType = expressionType;
    }
}
