using Sandbox.Scenes.TurnBasedCombatSample.Components;
using Sandbox.Scenes.TurnBasedCombatSample.Data;
using System.Diagnostics;

namespace Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;

internal sealed class HealAbilityEffect : IAbilityEffect
{
    public string EffectType => "Heal";

    public void Apply(EffectDefinition effect, AbilityEffectContext context)
    {
        var parameters = effect.Parameters;
        var amount = parameters.GetInt("Amount", 1);

        foreach (var target in context.Targets)
        {
            var statsComponent = target.GetComponent<CombatStatsComponent>();
            Debug.Assert(statsComponent is not null);
            statsComponent.Heal(amount);
        }
    }

    public bool CanApply(EffectDefinition effect, AbilityEffectContext context)
    {
        return true;
    }
}
