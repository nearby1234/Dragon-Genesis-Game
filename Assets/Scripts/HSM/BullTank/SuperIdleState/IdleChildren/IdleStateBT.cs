using System.Collections;
using UnityEngine;

public class IdleStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    public IdleStateBT(BullTankBoss stateMachine,SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter(); 
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetBool("Walk", false);
        stateMachine.StartCoroutine(TimeChangeMoveState());
       
    }
    public override void Update()
    {
        base.Update();
    }

    IEnumerator TimeChangeMoveState()
    {
        yield return new WaitForSeconds(2f);
        parent.ChangeChild<MoveStateBT>();
    }
}
