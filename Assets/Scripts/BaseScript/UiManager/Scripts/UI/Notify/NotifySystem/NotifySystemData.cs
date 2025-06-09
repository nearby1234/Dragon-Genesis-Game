using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public struct NotifyMessageMission<T> where T : MonoBehaviour
{
    public T uiElement;
    public QuestData questData;
    public string message;
}
public enum NotifyType
{
    StartGame,

}

[CreateAssetMenu(fileName = "NotifySystemData", menuName = "Scriptable Object/NotifySystemData", order = 1)]
public class NotifySystemData : ScriptableObject, IEnumKeyed<NotifyType>
{
    public NotifyType Key => notifyType;
    public NotifyType notifyType;

    [Header("Setting")]
    public float timeShowContentStartGame = 2f;
    public float timeAcceptFirstMission = 4f;
    public float timeFade;
    public float timeHideNotify;

    [Header("Content Show")]
    [TextArea(3,3)]
    public string StartGame = $"Chào mừng bạn đến thế giới Dragon Genesis!";
    [TextArea(3, 3)]
    public string AcceptFirstMission;

    [Header("Color")]
    // Khi thay đổi Color, Odin sẽ gọi UpdateKey()
    [OnValueChanged(nameof(UpdateKey))]
    public Color Color;
    public string hex;
    public string key;
    public string endColorKey;



#if UNITY_EDITOR
    private void UpdateKey()
    {
        // Sinh lại hex và key mỗi khi Color thay đổi trong inspector
        hex = $"#{ColorUtility.ToHtmlStringRGB(Color)}";
        key = $"<color={hex}>";
    }
#endif
}
