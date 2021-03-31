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
    Animator animator;
    void Start()
    {
        currentState = BossState.Idle;
        animator = GetComponent<Animator>();
    }

    public void ChangeState(BossState bs)
	{
        currentState = bs;
        RunState();
    }

    void RunState()
	{
        switch(currentState)
		{
            case BossState.Idle:
                animator.SetTrigger("Idle");
                break;
            case BossState.WideUp1:
                animator.SetTrigger("WideUp1");
                break;
            case BossState.WideUp2:
                animator.SetTrigger("WideUp2");
                break;
            case BossState.WideUp3:
                animator.SetTrigger("WideUp3");
                break;
            case BossState.WideUp4:
                animator.SetTrigger("WideUp4");
                break;
            case BossState.Attack1:
                animator.SetTrigger("Attack1");
                break;
            case BossState.Attack2:
                animator.SetTrigger("Attack2");
                break;
            case BossState.Attack3:
                animator.SetTrigger("Attack3");
                break;
            case BossState.Attack4:
                animator.SetTrigger("Attack4");
                break;
        }
	}

    public BossState CurrentState()
    {
        return currentState;
    }
}
