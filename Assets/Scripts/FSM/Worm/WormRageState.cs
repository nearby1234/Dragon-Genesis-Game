using System.Collections;
using UnityEngine;

public class WormRageState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine playTauntingLoop;
    public WormRageState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.RAGE);
        boss.Animator.Play("Angry",0,0f);
        boss.undergroundDuration = 1f;
        playTauntingLoop = boss.StartCoroutine(PlayTauntingLoop());
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.RAGE);
        if(playTauntingLoop != null)
        {
            boss.StopCoroutine(playTauntingLoop);
            playTauntingLoop = null;
        }

    }

    public override void Updates()
    {

    }

    private IEnumerator PlayTauntingLoop()
    {
        int loop = 0;
        while(loop <2)
        {
            boss.Animator.Play("Angry",0,0f);
            yield return new WaitUntil(() =>
            {
                var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName("Angry");
            });
            yield return new WaitUntil(() =>
            {
                var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                return (!info.IsName("Angry") || info.IsName("Angry") && info.normalizedTime > 1f);
            });
            loop++;
        }
        Debug.Log(loop);
        boss.Animator.SetInteger("Loop",loop);
       
        if(loop == 2)
        {
            boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
        }
       
    }

}
