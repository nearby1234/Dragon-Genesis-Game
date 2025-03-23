using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormUndergroundState : BaseState<WormBoss, WORMSTATE>
{
    private bool isLocated;
    private Coroutine waitUnderground;
    private Coroutine resetIdleGrace;

    public WormUndergroundState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.UNDERGROUND);
        boss.Animator.SetTrigger("Underground");
        boss.idleGraceActive = true;

        // Bắt đầu kiểm tra sau khi animation hoàn tất
        waitUnderground = boss.StartCoroutine(WaitPlayAnimationUnderground());
        resetIdleGrace = boss.StartCoroutine(ResetIdleGrace());
    }

    public override void Updates()
    {
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.UNDERGROUND);
        if (waitUnderground != null)
        {
            boss.StopCoroutine(waitUnderground);
            waitUnderground = null;
        }
        boss.Animator.ResetTrigger("Underground");
        if (resetIdleGrace != null)
        {
            boss.StopCoroutine(resetIdleGrace);
            resetIdleGrace = null;
        }
    }

    private IEnumerator WaitPlayAnimationUnderground()
    {
        // Chờ animation "GroundDiveIn" hoàn thành
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundDiveIn") &&
            boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        // Lựa chọn danh sách attack động theo phase hiện tại
        List<WormAttackData> attackDataList = boss.IsRageState ? boss.wormAttackDatasPhase2 : boss.wormAttackDatasPhase1;
        boss.currentAttackIndex = boss.GetRandomIndexAttackList();  
        float distanceOffset = attackDataList[boss.currentAttackIndex].stopDistance - 1.5f;
        float randomAngle = Random.Range(0f, 360f);
        Vector3 offset = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward * distanceOffset;
        // Di chuyển boss đến vị trí của player cộng offset (vị trí xuất hiện dynamic)
        Vector3 targetPos = boss.m_Player.transform.position + offset;
        boss.NavMeshAgent.Warp(targetPos);
        isLocated = true;
        boss.RequestStateTransition(WORMSTATE.EMERGE);

        // Kiểm tra nếu player vẫn trong phạm vi
        while (boss.PlayerInRange())
        {
            yield return new WaitForSeconds(0.2f);
        }

        // Nếu player ra khỏi phạm vi, chờ 1 giây rồi chuyển sang trạng thái EMERGE
        Debug.Log("Player left range. Transitioning to EMERGE state.");
        yield return new WaitForSeconds(1f);
        boss.MoveToRandomPosition();
        boss.RequestStateTransition(WORMSTATE.EMERGE);
    }
    private IEnumerator ResetIdleGrace()
    {
        yield return new WaitForSeconds(10f);
        boss.idleGraceActive = false;
    }
}
