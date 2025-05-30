using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Animation Clip", story: "[AgentAnimator] set play animation [name] [Count] times", category: "Action/Animation", id: "18b99009906139c9fffacb5610b97a13")]
public partial class PlayAnimationClipAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Name;
    [SerializeReference] public BlackboardVariable<Animator> AgentAnimator;
    [SerializeReference] public BlackboardVariable<int> Count;
    private int angryPlayCount = 0;
    private bool isPlaying;
    private int angryStateHash;




    protected override Status OnStart()
    {
        if (Name == null || AgentAnimator.Value == null)
            return Status.Failure;

        angryPlayCount = 0;
        angryStateHash = Animator.StringToHash(Name.Value);
        isPlaying = false;

        // kick off lần play đầu
        TriggerAngry();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var anim = AgentAnimator.Value;
        var info = anim.GetCurrentAnimatorStateInfo(0);

        // nếu đang trong state Angry và clip đã chạy xong
        if (isPlaying
            && info.shortNameHash == angryStateHash
            && info.normalizedTime >= 1.0f)
        {
            // đánh dấu đã play xong một lượt
            angryPlayCount++;
            isPlaying = false;
            Debug.Log($"Angry played {angryPlayCount}/{Count.Value}");

            // nếu còn lượt nữa thì trigger tiếp
            if (angryPlayCount < Count.Value)
            {
                TriggerAngry();
            }
            else
            {
                return Status.Success;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        angryPlayCount = 0;
        isPlaying = false;
    }
    private void TriggerAngry()
    {
        AgentAnimator.Value.SetTrigger("Angry");
        isPlaying = true;
    }
}

