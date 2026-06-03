using Engine.Debugging;
using Engine.ECS;

namespace Sandbox.Scenes.TurnBasedCombatSample.Components;

internal sealed class CombatantComponent : Component
{
    public enum ETeam
    {
        Player,
        Enemy
    }

    [DebugExpose]
    public ETeam Team { get; set; }

    [DebugExpose]
    public bool TookActionThisRound { get; set; }

    public CombatantComponent(ETeam team)
    {
        Team = team;
        TookActionThisRound = false;
    }
}
