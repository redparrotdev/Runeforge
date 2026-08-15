namespace Lore.Structure;

public class SayStep : DialogStep
{
    public string Character { get; protected set; }
    public string Text { get; protected set; }

    public SayStep(string character, string text)
    {
        Character = character;
        Text = text;
    }
}
