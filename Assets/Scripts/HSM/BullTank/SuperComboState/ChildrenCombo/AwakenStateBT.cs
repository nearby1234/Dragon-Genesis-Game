using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AwakenStateBT : State<BullTankBoss>
{
    private SuperComboState parent;

    public AwakenStateBT(BullTankBoss stateMachine, SuperComboState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.SetStateNavmeshAgent(3, true, true);
        stateMachine.Animator.SetTrigger("Angry");
        stateMachine.StartCoroutine(WaitPlayAnimationAngry());
        //stateMachine.StartCoroutine(PerformComboRoutine(2));
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.Animator.ResetTrigger("Idle");
        stateMachine.Animator.ResetTrigger("Angry");
        stateMachine.Animator.ResetTrigger("RunCombo");
        stateMachine.Animator.ResetTrigger("StartThrow");
    }

    private IEnumerator WaitPlayAnimationAngry()
    {
        // Chờ hết animation "Angry"
        yield return new WaitUntil(() =>
        {
            var info = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Angry") && info.normalizedTime >= 1f;
        });
        parent.NotifyAwakenStateComplete();

        
    }
  
}
