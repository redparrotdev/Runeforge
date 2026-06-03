using System;
using System.Collections.Generic;

namespace Sandbox.Scenes.TurnBasedCombatSample.Utils;

internal sealed class SimpleStateMachine<TState> where TState : struct, Enum
{
    public delegate void StateChangedHandler(TState previousState, TState currentState);

    private static readonly EqualityComparer<TState> _stateComparer = EqualityComparer<TState>.Default;

    public TState CurrentState { get; private set; }
    public TState PreviousState { get; private set; }

    public event StateChangedHandler OnStateChanged;

    public SimpleStateMachine(TState initialState)
    {
        CurrentState = initialState;
        PreviousState = initialState;
    }

    public void ChangeState(TState newState)
    {
        if (_stateComparer.Equals(newState, CurrentState)) return;

        PreviousState = CurrentState;
        CurrentState = newState;

        OnStateChanged?.Invoke(PreviousState, CurrentState);
    }

    public void GoToPreviousState()
    {
        ChangeState(PreviousState);
    }

    public bool IsInState(TState state)
    {
        return _stateComparer.Equals(CurrentState, state);
    }

    public bool IsInAnyState(params TState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (_stateComparer.Equals(CurrentState, states[i]))
                return true;
        }

        return false;
    }
}
