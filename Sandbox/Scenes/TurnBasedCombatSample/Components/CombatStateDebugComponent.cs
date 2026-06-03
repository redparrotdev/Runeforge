using Engine.Debugging;
using Engine.ECS;
using Sandbox.Scenes.TurnBasedCombatSample.Utils;

namespace Sandbox.Scenes.TurnBasedCombatSample.Components;

internal class CombatStateDebugComponent : Component
{
    private ECombatState _currentState;
    private ECombatState _previousState;

    [DebugExpose]
    public ECombatState CurrentState => _currentState;

    [DebugExpose]
    public ECombatState PreviousState => _previousState;

    public CombatStateDebugComponent(SimpleStateMachine<ECombatState> stateMachine)
    {
        _currentState = stateMachine.CurrentState;
        _previousState = stateMachine.PreviousState;

        stateMachine.OnStateChanged += (p, c) =>
        {
            _previousState = p;
            _currentState = c;
        };
    }
}
