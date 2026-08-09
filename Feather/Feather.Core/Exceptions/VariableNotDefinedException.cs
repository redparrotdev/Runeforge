namespace Feather.Core.Exceptions;

public sealed class VariableNotDefinedException : FeatherException
{
    public string VariableName { get; private set; }

    public VariableNotDefinedException(string variableName) : this(variableName, null)
    {
    }

    public VariableNotDefinedException(string variableName, Exception? innerException)
        : base($"Variable '{variableName}' is not defined.", innerException)
    {
        VariableName = variableName;
    }
}
