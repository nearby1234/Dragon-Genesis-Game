using System.Collections;
using UnityEngine;

public class AttackAxeStateBT : State<BullTankBoss>
{
    private SuperComboState parent;
    private bool m_HasStartedCombo;

    public AttackAxeStateBT(BullTankBoss stateMachine, SuperComboState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetTrigger("RunCombo");
        //stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
        stateMachine.StartCoroutine(PerformComboRoutine(2));
    }
    private IEnumerator PerformComboRoutine(int comboCount)
    {
        for (int i = 0; i < comboCount; i++)
        {
            stateMachine.BullTankAgent.ResetPath();
            stateMachine.Animator.SetBool("AttackCombo", false);
            stateMachine.BullTankAgent.isStopped = false;
            stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
            // Chờ đến khi vào tầm đánh
            yield return new WaitUntil(() =>
                stateMachine.Distance() &&
                stateMachine.DistanceWithPlayer() <= stateMachine.m_DistanceAttackSword
            );

            // Dừng, trigger đòn đánh
            //stateMachine.BullTankAgent.ResetPath();
            stateMachine.BullTankAgent.isStopped = true;
            stateMachine.Animator.SetBool("AttackCombo", true);

            // Chờ animation AttackCombo1 xong
            yield return new WaitUntil(() =>
            {
                var info = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName("AttackCombo1") && info.normalizedTime >= 1f;
            });
        }

        parent.NotifyAttackAxeStateComplete(true);

    }
}
