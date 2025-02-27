using UnityEngine;
using static MiniBoss;

public class AttackState : BaseState
{
    public AttackState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.state = ENEMYSTATE.ATTACK;
        miniBoss.Animator.Play("Attack02");
    }

    public override void Executed()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
