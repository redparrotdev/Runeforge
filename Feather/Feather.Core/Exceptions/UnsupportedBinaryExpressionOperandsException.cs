using Feather.Core.Structure;

namespace Feather.Core.Exceptions;

public sealed class UnsupportedBinaryExpressionOperandsException : FeatherException
{
    public BinaryOperatorType BinaryOperatorType { get; private set; }
    public Type? LeftOperandType { get; private set; }
    public Type? RightOperandType { get; private set; }

    public UnsupportedBinaryExpressionOperandsException(BinaryOperatorType binaryOperatorType, Type? leftOperandType, Type? rightOperandType)
        : this(binaryOperatorType, leftOperandType, rightOperandType, null)
    {
    }

    public UnsupportedBinaryExpressionOperandsException(
        BinaryOperatorType binaryOperatorType
        , Type? leftOperandType
        , Type? rightOperandType
        , Exception? innerException)
        : base(BuildMessage(binaryOperatorType, leftOperandType, rightOperandType), innerException)
    {
        BinaryOperatorType = binaryOperatorType;
        LeftOperandType = leftOperandType;
        RightOperandType = rightOperandType;
    }

    private static string BuildMessage(BinaryOperatorType binaryOperatorType, Type? leftOperandType, Type? rightOperandType)
    {
        return $"Unsupported binary expression operands: {leftOperandType?.Name ?? "null"} {binaryOperatorType} {rightOperandType?.Name ?? "null"}";
    }
}
