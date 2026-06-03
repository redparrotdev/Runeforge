using Engine.ECS;
using Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;
using Sandbox.Scenes.TurnBasedCombatSample.Components;
using Sandbox.Scenes.TurnBasedCombatSample.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Scenes.TurnBasedCombatSample.Systems;

internal sealed class AbilityExecutor
{
    private readonly AbilityEffectRegistry _effectRegistry;

    public AbilityExecutor(AbilityEffectRegistry effectRegistry)
    {
        _effectRegistry = effectRegistry;
    }

    public void ExecuteAbility(AbilityDefinition ability, Entity caster, IEnumerable<Entity> targets, IEnumerable<Entity> allEntities)
    {
        foreach (var effectDefinition in ability.Effects)
        {
            var effect = _effectRegistry.GetEffect(effectDefinition.Type);
            var effectTargets = ResolveEffectTargets(effectDefinition, caster, targets, allEntities);
            var effectContext = new AbilityEffectContext
            {
                Ability = ability,
                Caster = caster,
                Targets = effectTargets
            };

            if (effect.CanApply(effectDefinition, effectContext))
            {
                effect.Apply(effectDefinition, effectContext);
            }
        }
    }

    private static IEnumerable<Entity> ResolveEffectTargets(
        EffectDefinition effect
        , Entity caster
        , IEnumerable<Entity> targets
        , IEnumerable<Entity> allEntities)
    {
        var targetType = effect.Target;
        return targetType switch
        {
            AbilityEffectTarget.Caster => [caster],
            AbilityEffectTarget.Target => targets,
            AbilityEffectTarget.AllEnemies => GetAllOfTeam(CombatantComponent.ETeam.Enemy),
            AbilityEffectTarget.AllAllies => GetAllOfTeam(CombatantComponent.ETeam.Player),
            _ => throw new ArgumentOutOfRangeException($"Not supported effect target type '{targetType}'.")
        };

        IEnumerable<Entity> GetAllOfTeam(CombatantComponent.ETeam team)
        {
            return allEntities.Where(e => e.GetComponent<CombatantComponent>().Team == team);
        }
    }
}
