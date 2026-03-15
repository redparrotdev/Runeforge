using System.Xml;

namespace AtlasPacker.Helpers;

internal static class XmlHelper
{
    public static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        CloseInput = true,
        IgnoreWhitespace = true
    };

    public static readonly XmlWriterSettings XmlWriterSettings = new()
    {
        Indent = true,
        IndentChars = "  ",
        Async = true,
        NewLineChars = Environment.NewLine,
        CloseOutput = true
    };

    public static XmlReader CreateReader(string filePath)
    {
        return XmlReader.Create(filePath, XmlReaderSettings);
    }

    public static XmlWriter CreateWriter(string filePath)
    {
        return XmlWriter.Create(filePath, XmlWriterSettings);
    }
}
