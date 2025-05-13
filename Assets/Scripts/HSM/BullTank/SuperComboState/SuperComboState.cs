using UnityEngine;

public class SuperComboState : CompositeState<BullTankBoss>
{
    private bool m_IsAwakenStateComplete;
    private bool m_IsThrowStateComplete;
    private bool m_IsCoolDownStateComplete;
    private bool m_IsAttackAxeStateComplete;

    public SuperComboState(BullTankBoss stateMachine) : base(stateMachine)
    {
        AddChild(new AwakenStateBT(stateMachine, this));
        AddChild(new ThrowAxeStateBT(stateMachine, this));
        AddChild(new CoolDownStateBT(stateMachine, this));
        AddChild(new AttackAxeStateBT(stateMachine,this));  
        SetInitial<AwakenStateBT>();
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
    }
    public override void Update()
    {
        if(m_IsAwakenStateComplete && !m_IsAttackAxeStateComplete)
        {
            ChangeChild<AttackAxeStateBT>();
            return;
        }
        if (m_IsAttackAxeStateComplete && !m_IsThrowStateComplete)
        {
            ChangeChild<ThrowAxeStateBT>();
            return;
        }
        if(m_IsThrowStateComplete && !m_IsCoolDownStateComplete)
        {
            ChangeChild<CoolDownStateBT>();
            return;
        }
        base.Update();
    }

    public void NotifyAwakenStateComplete()
    {
        m_IsAwakenStateComplete = true;
    }
    public void NotifyAttackThrowStateComplete(bool isComplete)
    {
        m_IsThrowStateComplete = isComplete;
    }
    public void NotifyCoolDownStateComplete(bool isComplete)
    {
        m_IsCoolDownStateComplete = isComplete;
    }
    public void NotifyAttackAxeStateComplete(bool isComplete)
    {
        m_IsAttackAxeStateComplete = isComplete;
    }

}
