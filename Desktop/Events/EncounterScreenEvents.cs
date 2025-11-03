using Engine.Events;

namespace Desktop.Events;
public static partial class GameEvents
{
    public class EncounterOptionSelected : BaseEvent
    {
        public readonly int OptionIndex;

        public EncounterOptionSelected(int optionIndex)
        {
            OptionIndex = optionIndex;
        }
    }
}
