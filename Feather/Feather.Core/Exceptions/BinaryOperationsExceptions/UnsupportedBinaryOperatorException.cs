using Feather.Core.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feather.Core.Exceptions.BinaryOperationsExceptions;

public sealed class UnsupportedBinaryOperatorException : FeatherException
{
    public BinaryOperatorType BinaryOperatorType { get; private set; }

    public UnsupportedBinaryOperatorException(BinaryOperatorType binaryOperatorType) 
        : this(binaryOperatorType, null)
    {
    }

    public UnsupportedBinaryOperatorException(BinaryOperatorType binaryOperatorType, Exception? innerException) 
        : base($"Unsupported binary operator: {binaryOperatorType}", innerException)
    {
        BinaryOperatorType = binaryOperatorType;
    }

}
