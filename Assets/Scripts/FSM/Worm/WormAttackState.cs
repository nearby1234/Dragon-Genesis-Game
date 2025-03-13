using UnityEngine;
using System.Collections;

public class WormAttackState : BaseState<WormBoss, WORMSTATE>
{
    public WormAttackState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.ATTACK);
        Debug.Log("WormBoss: Attacking using index " + boss.currentAttackIndex);
        // L?y tên animation t?n công d?a trên currentAttackIndex
        string attackAnim = boss.wormAttackDatas[boss.currentAttackIndex].animationName;
        boss.Animator.Play(attackAnim);
        boss.StartCoroutine(WaitForAttack());
    }

    private IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(1.5f); // gi? s? attack animation kéo dài 1.5s
        // Sau khi t?n công xong, tr? l?i tr?ng thái Idle ?? l?p l?i
        boss.RequestStateTransition(WORMSTATE.IDLE);
    }

    public override void Updates() { }

    public override void Exit()
    {
        Debug.Log("WormBoss: Finishing attack.");
    }
}
