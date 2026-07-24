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
}
