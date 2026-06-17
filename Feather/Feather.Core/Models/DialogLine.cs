namespace Feather.Core.Models;

public sealed class DialogLine
{
    public readonly string CharacterName;
    public readonly string Text;

    public DialogLine(string characterName, string text)
    {
        CharacterName = characterName;
        Text = text;
    }
}
