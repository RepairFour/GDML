using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    WideUp1,
    WideUp2,
    WideUp3,
    WideUp4,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
}

public class BossStateMachine : MonoBehaviour
{
    BossState currentState;
    void Start()
    {
        currentState = BossState.Idle;
    }

    public void ChangeState(BossState bs)
	{
        currentState = bs;
        RunState();
    }

    void RunState()
	{

	}

    public BossState CurrentState()
    {
        return currentState;
    }
}
