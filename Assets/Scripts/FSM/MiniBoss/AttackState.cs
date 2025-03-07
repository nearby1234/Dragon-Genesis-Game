using System.Collections;
using UnityEngine;
using static MiniBoss;

public class AttackState : BaseState
{
    private float m_Timer;
    private float m_AttackTime = 3f;         // Thời gian attack
    private float m_RotationDelay = 0.5f;      // Khoảng delay sau attack trước khi xoay
    private Quaternion lockedRotation;       // Lưu hướng ban đầu khi attack

    public AttackState(MiniBoss miniBoss, FSM fsm) : base(miniBoss, fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.state = ENEMYSTATE.ATTACK;
        
        // Lưu lại hướng hiện tại của Boss khi bắt đầu tấn công
        lockedRotation = miniBoss.transform.rotation;
        miniBoss.NavmeshAgent.isStopped = true;
        // Chạy animation Attack
        miniBoss.StartCoroutine(miniBoss.PlayAnimationAttack());
    }

    public override void Executed()
    {
        miniBoss.Rotation();
        if(miniBoss.Distance()> miniBoss.StopDistance)
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.DETEC);
        }

        //m_Timer += Time.deltaTime;

        //if (m_Timer < m_AttackTime)
        //{
        //    // Trong suốt thời gian attack, giữ nguyên hướng ban đầu
        //    miniBoss.transform.rotation = lockedRotation;
        //}
        //else if (m_Timer < m_AttackTime + m_RotationDelay)
        //{
        //    // Sau attack xong, chờ thêm delay - vẫn giữ nguyên hướng
        //}
        //else
        //{
        //    // Sau khi delay xong, cập nhật hướng xoay về phía player

        //    // Nếu muốn reset cho lần attack tiếp theo:
        //    lockedRotation = miniBoss.transform.rotation;
        //    m_Timer = 0f;
        //}
        //Debug.Log(m_Timer);
    }

    public override void Exit()
    {
        // Xử lý khi thoát trạng thái nếu cần
    }
}
