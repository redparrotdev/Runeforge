using Sandbox.Scenes.TurnBasedCombatSample.Components;
using Sandbox.Scenes.TurnBasedCombatSample.Data;

namespace Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;

internal sealed class BaseAttackComponent : AbilityComponent
{
    public BaseAttackComponent() : base(CreateAttackDefinition())
    {
    }

    private static AbilityDefinition CreateAttackDefinition()
    {
        return new AbilityDefinition
        {
            Id = "Attack",
            Name = "Attack",
            Description = "Basic attack",
            Target = new TargetDefinition
            {
                Type = "Enemy"
            },
            Effects = [
                new EffectDefinition
                {
                    Type = "Damage",
                    Target = "Target"
                }
            ]
        };
    }
}
