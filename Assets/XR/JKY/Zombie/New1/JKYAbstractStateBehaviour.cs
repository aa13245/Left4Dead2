using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JKYAbstractStateBehaviour<StateType> : MonoBehaviour
{
    // Start is called before the first frame update
    [field: SerializeField]
    public StateType State
    {
        get; protected set;
    }
    public delegate void StateChangeEvent(StateType OldState, StateType NewState);
    public StateChangeEvent OnStateChange;

    public virtual void ChangeState(StateType NewState)
    {
        OnStateChange?.Invoke(State, NewState);
        State = NewState;
    }
}
