using System.Collections.Generic;
using System.Diagnostics;

namespace Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;

internal sealed class AbilityEffectRegistry
{
    private readonly Dictionary<string, IAbilityEffect> _effects = new();

    public void RegisterEffect(string name, IAbilityEffect effect)
    {
        _effects[name] = effect;
    }

    public IAbilityEffect GetEffect(string type)
    {
        _effects.TryGetValue(type, out var effect);
        Debug.Assert(effect is not null);
        return effect;
    }
}
