namespace Feather.Core.Exceptions;

public sealed class UnsupportedLabelExpressionTypeException : FeatherException
{
    public Type ExpressionType { get; private set; }

    public UnsupportedLabelExpressionTypeException(Type expressionType) : this(expressionType, null)
    {
    }

    public UnsupportedLabelExpressionTypeException(Type expressionType, Exception? innerException)
        : base($"Unsupported label expression type: {expressionType.FullName}", innerException)
    {
        ExpressionType = expressionType;
    }
}
