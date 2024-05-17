using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : State<T>
{
    protected StateMachine<T> stateMachine;

    public void SetStateMachine(StateMachine<T> machine)
    {
        stateMachine = machine ?? throw new ArgumentNullException(nameof(machine));
    }

    public virtual void Enter() 
    { 
        Debug.Log(this.ToString()); 
    }
    public virtual void Update() { }
    public virtual void Exit() { }
}


public class StateMachine<T> where T : State<T>
{
    private T currentState;
    public T CurrentState => currentState;

    public void StateUpdate()
    {
        currentState?.Update();
    }

    public void SetState(T state)
    {
        currentState?.Exit();

        currentState = state ?? throw new ArgumentNullException(nameof(state));
        currentState.SetStateMachine(this);

        currentState.Enter();
    }

    public IEnumerator ChangeState(float waitTime, T state)
    {
        yield return new WaitForSeconds(waitTime);

        SetState(state);
    }
}