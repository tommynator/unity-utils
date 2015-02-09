using System;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
    void OnDebugGui();
}

public class StateMachine : StateMachine<int> { }

public class StateMachine<T> where T : struct, IConvertible
{
    private Dictionary<T, IState> registeredStates;

    public event Action<T> OnPrepareState = delegate {};
    
    public T? currentState;
    
    public T CurrentState { get { return currentState.GetValueOrDefault(); } }
    
    public StateMachine()
    {
        registeredStates = new Dictionary<T, IState>();
        currentState = null;
    }
    
    public void SetState(T stateId)
    {
        if (ContainsState(stateId))
        {
            if (!currentState.HasValue || !stateId.Equals(currentState.Value))
            {
                ExitState();
                currentState = stateId;
                OnPrepareState(stateId);
                registeredStates[stateId].OnEnter();
            }
        }
        else
        {
            throw new Exception("Unsupported state of type: " + stateId);
        }
    }
    
    public void ExitState()
    {
        if (currentState.HasValue)
        {
            registeredStates[currentState.Value].OnExit();
            currentState = null;
        }
    }
    
    public void Reset()
    {
        ExitState();
        
        registeredStates.Clear();
    }
    
    public void RegisterState(T stateId, IState state)
    {
        if (!registeredStates.ContainsKey(stateId))
        {
            registeredStates.Add(stateId, state);
        }
        else
        {
            throw new Exception("StateMachine::RegisterState - state [" + stateId + "] already exists in the current registry");
        }
    }
    
    public bool ContainsState(T stateId)
    {
        return registeredStates.ContainsKey(stateId);
    }
    
    public void Update()
    {
        if(currentState.HasValue)
        {
            registeredStates[currentState.Value].OnUpdate();
        }
    }
    
    public void OnDebugGUI()
    {
        if (currentState.HasValue && DebugInterface.IsEnabled)
        {
            registeredStates[currentState.Value].OnDebugGui();
        }
    }
}