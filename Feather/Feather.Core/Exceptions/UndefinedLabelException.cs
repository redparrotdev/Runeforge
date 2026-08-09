namespace Feather.Core.Exceptions;

public sealed class UndefinedLabelException : FeatherException
{
    public string Label { get; private set; }

    public UndefinedLabelException(string label) : this(label, null)
    {
    }

    public UndefinedLabelException(string label, Exception? innerException)
        : base($"Label '{label}' is not defined.", innerException)
    {
        Label = label;
    }
}
