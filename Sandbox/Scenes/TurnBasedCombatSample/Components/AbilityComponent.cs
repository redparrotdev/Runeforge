using Engine.ECS;
using Sandbox.Scenes.TurnBasedCombatSample.Data;

namespace Sandbox.Scenes.TurnBasedCombatSample.Components;

internal class AbilityComponent : Component
{
    public readonly AbilityDefinition AbilityDefinition;

    public AbilityComponent(AbilityDefinition abilityDefinition)
    {
        AbilityDefinition = abilityDefinition;
    }
}
