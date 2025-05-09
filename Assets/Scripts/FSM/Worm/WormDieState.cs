using System.Collections;
using UnityEngine;

public class WormDieState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitBossDie;
    public WormDieState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.DIE);
        boss.Animator.CrossFade("Die", 0.1f);
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.BOSS_STATE_CURRENT, WORMSTATE.DIE);
        }
        if(AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("WormBossDie");
        }
        boss.SetHideCollider();
        waitBossDie = boss.StartCoroutine(WaitBossDie());
        
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.DIE);
        if (waitBossDie != null)
        {
            boss.StopCoroutine(waitBossDie);
            waitBossDie = null;
        }
    }
    public override void Updates()
    {
        
    }

    private IEnumerator WaitBossDie()
    {
        yield return new WaitUntil(() =>
        {
            var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Die");
        });
        yield return new WaitUntil(() =>
        {
            var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
            return (!info.IsName("Die")) || (info.IsName("Die") && info.normalizedTime >= 1f);
        });
        boss.StartCoroutine(boss.dissovleController.DissolveCo());
        if(AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("DissolveSound");
        }    
    }
}
