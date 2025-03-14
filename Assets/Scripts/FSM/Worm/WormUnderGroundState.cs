﻿using UnityEngine;
using System.Collections;

public class WormUndergroundState : BaseState<WormBoss, WORMSTATE>
{
    private bool isLocated;
    private Coroutine waitUnderground;

    public WormUndergroundState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.UNDERGROUND);
        boss.Animator.Play(boss.undergroundAnimation, 0, 0f);
        boss.Animator.SetTrigger("Underground");

        // Bắt đầu kiểm tra sau khi animation hoàn tất
        waitUnderground = boss.StartCoroutine(WaitPlayAnimationUnderground());
    }

    public override void Updates()
    {
        // Không cần làm gì trong Update, tất cả xử lý trong coroutine
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.UNDERGROUND);
        boss.StopCoroutine(waitUnderground);
        boss.Animator.ResetTrigger("Underground");
    }

    private IEnumerator WaitPlayAnimationUnderground()
    {
        // Chờ animation "GroundDiveIn" hoàn thành
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundDiveIn") &&
            boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        Debug.Log("Underground animation finished.");

        // Kiểm tra nếu player vẫn trong phạm vi
        while (boss.PlayerInRange())
        {
            if (!isLocated)
            {
                // Di chuyển tới vị trí của player + 7 đơn vị ở trục Z
                Vector3 targetPos = boss.m_Player.transform.position;
                targetPos.z += 7f;
                boss.NavMeshAgent.Warp(targetPos);
                isLocated = true;
                Debug.Log("Boss teleported near player.");
            }

            yield return new WaitForSeconds(0.2f); // Kiểm tra lại mỗi 0.2 giây
        }

        // Nếu player ra khỏi phạm vi, chờ 1 giây rồi chuyển sang trạng thái EMERGE
        Debug.Log("Player left range. Transitioning to EMERGE state.");
        yield return new WaitForSeconds(1f);
        boss.RequestStateTransition(WORMSTATE.EMERGE);
    }
}
