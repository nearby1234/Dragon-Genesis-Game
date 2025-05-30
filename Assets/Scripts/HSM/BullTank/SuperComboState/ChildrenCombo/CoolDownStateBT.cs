using System.Collections;
using UnityEngine;

public class CoolDownStateBT : State<BullTankBoss>
{
    private SuperComboState parent;
    public CoolDownStateBT(BullTankBoss stateMachine, SuperComboState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetTrigger("Idle");
        stateMachine.Animator.SetBool("Walk",false);
        stateMachine.StartCoroutine(WaitTranferAttackState());
    }
    public override void Exit()
    {
        stateMachine.Animator.ResetTrigger("Idle");
        base.Exit();
    }
    private IEnumerator WaitTranferAttackState()
    {
        yield return new WaitForSeconds(4f);
        RaiseComplete();
    }

}
