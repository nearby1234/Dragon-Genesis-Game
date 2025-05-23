using System.Collections;
using UnityEngine;

public class AttackThrowAxeBT : State<BullTankBoss>
{
    private SuperAttackStateBT parent;
    private bool m_IsThrow;
    private bool m_HasLockedDirection;
    private Vector3 m_ThrowDirection;
    public AttackThrowAxeBT(BullTankBoss stateMachine, SuperAttackStateBT parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetTrigger("StartThrow");
    }
    public override void Update()
    {
       if(stateMachine.Distance() && !m_HasLockedDirection)
        {
            stateMachine.Rotation();

            if (!m_IsThrow)
            {
                stateMachine.StartCoroutine(ThrowDuration());
                m_IsThrow = true;
            }
        }
        base.Update();
    }

    IEnumerator ThrowDuration()
    {
        yield return new WaitForSeconds(4f);
        m_HasLockedDirection = true;
        m_ThrowDirection = stateMachine.transform.forward;
        stateMachine.transform.rotation = Quaternion.LookRotation(m_ThrowDirection);
        stateMachine.Animator.SetTrigger("EndThrow");
        //m_IsThrow = true;
        yield return new WaitUntil(() => stateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03_SpawnAxe"));
        yield return new WaitUntil(() =>
        {
            stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            return (stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        });
        parent.NotifyAttackThrowStateComplete();
    }
}
