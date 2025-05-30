using System.Collections;
using UnityEngine;

public class IdleStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    private Coroutine m_TimeChangeMoveState;
    public IdleStateBT(BullTankBoss stateMachine,SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter(); 
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetBool("Walk", false);
        m_TimeChangeMoveState =stateMachine.StartCoroutine(TimeChangeMoveState());
       
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        if(m_TimeChangeMoveState != null)
            stateMachine.StopCoroutine(m_TimeChangeMoveState);
    }

    IEnumerator TimeChangeMoveState()
    {
        yield return new WaitForSeconds(2f);
        parent.ChangeChild<MoveStateBT>();
    }
}
