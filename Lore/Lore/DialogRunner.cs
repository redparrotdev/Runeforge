using Lore.Structure;

namespace Lore;

public sealed class DialogRunner
{
    private IEnumerator<DialogStep> _dialogEnumerator;

    public DialogRunner(Dialog dialog)
    {
        _dialogEnumerator = dialog.Start();
    }

    public DialogStep? Advance()
    {
        if (_dialogEnumerator.Current is ChoiceStep choice
            && !choice.IsOptionSelected)
        {
            return _dialogEnumerator.Current;
        }

        if (!_dialogEnumerator.MoveNext()) return null;

        var step = _dialogEnumerator.Current;
        if (step is RouteStep route)
        {
            _dialogEnumerator = route.Route;
            return Advance();
        }

        return step;
    }
}
