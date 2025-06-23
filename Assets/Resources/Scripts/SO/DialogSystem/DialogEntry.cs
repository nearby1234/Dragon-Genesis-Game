using UnityEngine;

public enum DialogState
{
    Default = 0,
    Accept,
    Deny,
    ChooseReward,
    Reward,
    Wait,
    Exit,
}
[System.Serializable]
public class DialogEntry 
{
    public DialogState state;
    [TextArea(10, 10)]
    public string content;
    [TextArea(3, 10)]
    public string contentFriendly;
    [TextArea(3, 10)]
    public string contentUnFriendly;
    public AudioClip voiceClip;
}
