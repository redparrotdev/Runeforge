namespace Feather.Core.Exceptions;

public sealed class UnsupportedStatementTypeException : FeatherException
{
    public Type StatementType { get; private set; }

    public UnsupportedStatementTypeException(Type statementType) : this(statementType, null)
    {
    }

    public UnsupportedStatementTypeException(Type statementType, Exception? innerException)
        : base($"Unsupported statement type: {statementType.FullName}", innerException)
    {
        StatementType = statementType;
    }
}
