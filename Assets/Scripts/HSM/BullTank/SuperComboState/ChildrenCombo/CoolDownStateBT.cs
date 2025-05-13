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
        stateMachine.StartCoroutine(WaitTranferAttackState());
    }
    private IEnumerator WaitTranferAttackState()
    {
        yield return new WaitForSeconds(2f);
        parent.NotifyAttackAxeStateComplete(false);
    }

}
