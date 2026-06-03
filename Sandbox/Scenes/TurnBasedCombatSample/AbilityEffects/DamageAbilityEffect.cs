using Sandbox.Scenes.TurnBasedCombatSample.Components;
using Sandbox.Scenes.TurnBasedCombatSample.Data;
using System;
using System.Diagnostics;

namespace Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;

internal class DamageAbilityEffect : IAbilityEffect
{
    public string EffectType => "Damage";

    public void Apply(EffectDefinition effect, AbilityEffectContext context)
    {
        var parameters = effect.Parameters;
        var damageMultiplier = parameters.GetFloat("Multiplier", 1f);

        var casterStats = context.Caster.GetComponent<CombatStatsComponent>();
        Debug.Assert(casterStats is not null);

        var damageToDeal = (int)MathF.Floor(casterStats.Attack * damageMultiplier);

        foreach (var target in context.Targets)
        {
            var statsComponent = target.GetComponent<CombatStatsComponent>();
            Debug.Assert(statsComponent is not null);

            statsComponent.TakeDamage(damageToDeal);
        }
    }

    public bool CanApply(EffectDefinition effect, AbilityEffectContext context)
    {
        return true;
    }
}
