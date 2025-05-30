using UnityEngine;

public enum NameState
{
    UnKnown = 0,
    RunStart,
    Attack2End,
    Attack01Axe,
    Anim_SpawnAxe,
    Anim_Angry,
}
public class AnimationCompleteBehaviour : StateMachineBehaviour
{
    public NameState nameState;
    private bool _fired;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fired = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_fired && stateInfo.normalizedTime >= 0.9f)
        {
            _fired = true;
            var bulltankBoss = animator.GetComponent<BullTankBoss>();
            if (bulltankBoss != null)
            {
                bulltankBoss.HandleAnimationComplete(nameState);
            }
        }

    }
}
