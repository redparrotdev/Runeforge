using Engine.ECS;
using Engine.ECS.Querying.Extensions;
using Sandbox.Scenes.TurnBasedCombatSample.Components;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Scenes.TurnBasedCombatSample.Systems;

internal sealed class TurnSystem
{
    private readonly Scene _scene;
    private readonly List<Entity> _aliveCombatants;
    private readonly Queue<Entity> _turnQueue;
    private int _currentRound;

    public Entity CurrentCombatant => _turnQueue.Count > 0 ? _turnQueue.Peek() : null;
    public int CurrentRound => _currentRound;

    public TurnSystem(Scene scene)
    {
        _scene = scene;
        _aliveCombatants = new List<Entity>(8);
        _turnQueue = new Queue<Entity>(8);
    }

    public void InitializeCombat()
    {
        var combatans = _scene.Query(
            builder => builder.WithAll<CombatStatsComponent, CombatantComponent>());

        _aliveCombatants.AddRange(combatans);

        _currentRound = 1;

        InitializeTurnQueue();
    }

    public void NextTurn()
    {
        if (_turnQueue.Count == 0)
        {
            return;
        }

        var current = _turnQueue.Dequeue();
        var combatantComponent = current.GetComponent<CombatantComponent>();
        combatantComponent.TookActionThisRound = true;
    }

    public void NextRound()
    {
        InitializeTurnQueue();
        ResetCombatansActions();
        _currentRound++;
    }

    private void InitializeTurnQueue()
    {
        _turnQueue.Clear();

        var orderedCombatants = _aliveCombatants
            .OrderByDescending(c => c.GetComponent<CombatStatsComponent>().Speeed);

        foreach (var combatant in orderedCombatants)
        {
            _turnQueue.Enqueue(combatant);
        }
    }

    private void ResetCombatansActions()
    {
        foreach (var combatant in _aliveCombatants)
        {
            var combatantComponent = combatant.GetComponent<CombatantComponent>();
            combatantComponent.TookActionThisRound = false;
        }
    }
}
