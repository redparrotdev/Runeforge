using Engine.ECS;
using Sandbox.Scenes.TurnBasedCombatSample.Data;
using System.Collections.Generic;

namespace Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;

internal sealed class AbilityEffectContext
{
    public Entity Caster { get; set; }
    public IEnumerable<Entity> Targets { get; set; }
    public AbilityDefinition Ability { get; set; }
}

internal interface IAbilityEffect
{
    string EffectType { get; }

    bool CanApply(EffectDefinition effect, AbilityEffectContext context);

    void Apply(EffectDefinition effect, AbilityEffectContext context);
}
