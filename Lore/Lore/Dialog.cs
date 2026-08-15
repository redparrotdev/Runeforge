using Lore.Structure;

namespace Lore;

public abstract class Dialog
{
    public abstract IEnumerator<DialogStep> Start();

    public virtual SayStep Say(string character, string text) => new SayStep(character, text);

    public virtual ChoiceOption? Option(string text, out string id, Func<ChoiceOption, bool> condition)
    {
        id = Guid.NewGuid().ToString();
        var option = new ChoiceOption(id, text);
        if (!condition(option)) return null;

        return option;
    }

    public virtual ChoiceStep Choice(params ChoiceOption?[] options)
    {
        return new ChoiceStep([..options.Where(o => o is { }).Cast<ChoiceOption>()]);
    }

    public virtual RouteStep Route(IEnumerator<DialogStep> route) => new RouteStep(route);
    public virtual RouteStep Route(Dialog dialog) => new RouteStep(dialog.Start());
}
