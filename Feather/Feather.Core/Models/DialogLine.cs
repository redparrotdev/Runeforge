namespace Feather.Core.Models;

public sealed class DialogLine
{
    public readonly string CharacterName;
    public readonly string Text;

    public List<Choice> Choices { get; } = [];

    public DialogLine(string characterName, string text)
    {
        CharacterName = characterName;
        Text = text;
    }

    public sealed record Choice(string Text);
}
