using Feather.Core.Exceptions.BinaryOperationsExceptions;
using Feather.Core.Structure;
using System.Runtime.CompilerServices;

namespace Feather.Core.Helpers;

public static class BinaryExpressionEvaluationHelper
{
    public static bool InAnyOrder<TLeft, TRight>(object? left, object? right, out StrongBox<TLeft> outLeft, out StrongBox<TRight> outRight)
    {
        if (left is StrongBox<TLeft> leftValue && right is StrongBox<TRight> rightValue)
        {
            outLeft = leftValue;
            outRight = rightValue;
            return true;
        }

        if (left is StrongBox<TRight> leftValueSwapped && right is StrongBox<TLeft> rightValueSwapped)
        {
            outLeft = rightValueSwapped;
            outRight = leftValueSwapped;
            return true;
        }

        outLeft = default!;
        outRight = default!;
        return false;
    }
    
    public static object? EvaluatePlusOperatorTypeBinaryExpression(object? leftValue, object? rightValue)
    {
        // "Hello " + "World" = "Hello World"
        if (leftValue is StrongBox<string> leftStr && rightValue is StrongBox<string> rightStr)
        {
            return new StrongBox<string>(leftStr.Value + rightStr.Value);
        }
        // 5 + 10 = 15
        if (leftValue is StrongBox<float> leftFloat && rightValue is StrongBox<float> rightFloat)
        {
            return new StrongBox<float>(leftFloat.Value + rightFloat.Value);
        }
        // true + false = 1
        if (leftValue is StrongBox<bool> leftBool && rightValue is StrongBox<bool> rightBool)
        {
            return new StrongBox<float>(Convert.ToSingle(leftBool.Value) + Convert.ToSingle(rightBool.Value));
        }
        // 5 + true = 6
        if (InAnyOrder<float, bool>(leftValue, rightValue, out var floatValue, out var boolValue))
        {
            return new StrongBox<float>(floatValue.Value + Convert.ToSingle(boolValue.Value));
        }

        throw new UnsupportedBinaryExpressionOperandsException(BinaryOperatorType.Plus, leftValue?.GetType(), rightValue?.GetType());
    }

    public static object? EvaluateMinusOperatorTypeBinaryExpression(object? leftValue, object? rightValue)
    {
        // "Hello World!" - "World" = "Hello !"
        if (leftValue is StrongBox<string> leftString && rightValue is StrongBox<string> rightString)
        {
            return new StrongBox<string>(leftString.Value!.Replace(rightString.Value!, string.Empty));
        }
        // 5 - 10 = -5
        if (leftValue is StrongBox<float> leftFloat && rightValue is StrongBox<float> rightFloat)
        {
            return new StrongBox<float>(leftFloat.Value - rightFloat.Value);
        }
        // true - false = 1
        if (leftValue is StrongBox<bool> leftBool && rightValue is StrongBox<bool> rightBool)
        {
            return new StrongBox<float>(Convert.ToSingle(leftBool.Value) - Convert.ToSingle(rightBool.Value));
        }
        // 5 - true = 4
        if (leftValue is StrongBox<float> leftFloat2 && rightValue is StrongBox<bool> rightBool2)
        {
            return new StrongBox<float>(leftFloat2.Value - Convert.ToSingle(rightBool2.Value));
        }
        // true - 5 = -4
        if (leftValue is StrongBox<bool> leftBool2 && rightValue is StrongBox<float> rightFloat2)
        {
            return new StrongBox<float>(Convert.ToSingle(leftBool2.Value) - rightFloat2.Value);
        }

        throw new UnsupportedBinaryExpressionOperandsException(BinaryOperatorType.Minus, leftValue?.GetType(), rightValue?.GetType());
    }

    public static object? EvaluateMultiplyOperatorTypeBinaryExpression(object? leftValue, object? rightValue)
    {
        // "Hello " * 3 = "Hello Hello Hello "
        if (leftValue is StrongBox<string> leftString && rightValue is StrongBox<float> rightFloat)
        {
            return new StrongBox<string>(string.Concat(Enumerable.Repeat(leftString.Value!, (int)rightFloat.Value)));
        }
        // 3 * 3 = 9
        if (leftValue is StrongBox<float> leftFloat && rightValue is StrongBox<float> rightFloat2)
        {
            return new StrongBox<float>(leftFloat.Value * rightFloat2.Value);
        }
        // true * false = 0
        if (leftValue is StrongBox<bool> leftBool && rightValue is StrongBox<bool> rightBool)
        {
            return new StrongBox<float>(Convert.ToSingle(leftBool.Value) * Convert.ToSingle(rightBool.Value));
        }
        // 5 * true = 5
        if (InAnyOrder<float, bool>(leftValue, rightValue, out var floatValue, out var boolValue))
        {
            return new StrongBox<float>(floatValue.Value * Convert.ToSingle(boolValue.Value));
        }

        throw new UnsupportedBinaryExpressionOperandsException(BinaryOperatorType.Multiply, leftValue?.GetType(), rightValue?.GetType());
    }

#pragma warning disable S3776 // 16 cognitive complexity is almost 15
    public static object? EvaluateDivideOperatorTypeBinaryExpression(object? leftValue, object? rightValue)
    {
        const string divideByZeroExceptionMessage = "Division by zero is not allowed.";

        if (leftValue is StrongBox<float> leftFloat && rightValue is StrongBox<float> rightFloat)
        {
            return rightFloat.Value.Equals(0f)
                ? throw new DivideByZeroException(divideByZeroExceptionMessage)
                : new StrongBox<float>(leftFloat.Value / rightFloat.Value);
        }
        if (leftValue is StrongBox<bool> leftBool && rightValue is StrongBox<bool> rightBool)
        {
            return !rightBool.Value
                ? throw new DivideByZeroException(divideByZeroExceptionMessage)
                : new StrongBox<float>(Convert.ToSingle(leftBool.Value) / Convert.ToSingle(rightBool.Value));
        }
        if (leftValue is StrongBox<float> leftFloat2 && rightValue is StrongBox<bool> rightBool2)
        {
            return !rightBool2.Value
                ? throw new DivideByZeroException(divideByZeroExceptionMessage)
                : new StrongBox<float>(leftFloat2.Value / Convert.ToSingle(rightBool2.Value));
        }
        if (leftValue is StrongBox<bool> leftBool2 && rightValue is StrongBox<float> rightFloat2)
        {
            return rightFloat2.Value.Equals(0f)
                ? throw new DivideByZeroException(divideByZeroExceptionMessage)
                : new StrongBox<float>(Convert.ToSingle(leftBool2.Value) / rightFloat2.Value);
        }

        throw new UnsupportedBinaryExpressionOperandsException(BinaryOperatorType.Divide, leftValue?.GetType(), rightValue?.GetType());
    }
#pragma warning restore S3776
}
