namespace Lore.Structure;

public class ChoiceOption
{
    public readonly string Id;
    public string Text { get; protected set; }

    public ChoiceOption(string id, string text)
    {
        Id = id;
        Text = text;
    }
}

public class ChoiceStep : DialogStep
{
    public IReadOnlyCollection<ChoiceOption> Options { get; protected set; }
    
    public bool IsOptionSelected { get; protected set; }
    public string SelectedOptionId { get; protected set; } = null!;

    public ChoiceStep(params ChoiceOption[] options)
    {
        Options = options;
    }

    public virtual void SelectOption(ChoiceOption option)
    {
        IsOptionSelected = true;
        SelectedOptionId = option.Id;
    }

    public virtual bool SelectedIs(string id)
    {
        return IsOptionSelected && SelectedOptionId == id;
    }

    public virtual ChoiceOption OptionAtIndex(int index)
    {
        return Options.ElementAt(index);
    }
}
