using System;
using UnityEngine;

public enum DialogMission
{
    Unknown = 0,
    DialogMissionFirst,
    DialogMissionSecond,
    DialogMissionThird,
    DialogMissionFourth,

}

[CreateAssetMenu(fileName = "DialogSystemSO", menuName = "Dialog System/DialogSystemSO")]
public class DialogSystemSO : ScriptableObject , IEnumKeyed<DialogMission>
{
    public DialogMission Key => dialogMission;
    public DialogMission dialogMission;

    [Header("Dialog Content")]
    [TextArea(3, 10)]
    public string DialogTitle;
    [TextArea(20, 10)]
    public string DialogContent;
    [TextArea(3, 10)]
    public string contentAcceptButton;
    public bool isClickAcceptButton;
    [TextArea(3, 10)]
    public string dialogClickAcceptButton;
    public bool isClickDenyButton;
    [TextArea(3, 10)]
    public string contentDenyButton;
    [TextArea(3, 10)]
    public string dialogClickDenyButton;
    [Header("Color")]
    public Color Color;
    public string hex;
    public string key;
    [Header("SpeedText")]
    public float speedText = 0.05f;
    public string keySpeedText;
    [Header("SizeText")]
    public float sizeText = 0.05f;
    public string keySizeText;
    public string keyEndSizeText = "</size>";
    [Header("Transition")]
    public string keyTransition = "<fade>";
    public string keyEndTransition = "</fade>";

    [Header("Event")]
    public Action OnClickAcceptButton;
    public Action OnClickDenyButton;

    private string _backupJson;


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Sinh lại hex và key mỗi khi Color thay đổi trong inspector
        hex = $"#{ColorUtility.ToHtmlStringRGB(Color)}";
        key = $"<color={hex}>";
        keySpeedText = $"<speed={speedText}>";
        keySizeText = $"<size={sizeText}>";
    }
#endif

    public void BackupData()
    {
        _backupJson = JsonUtility.ToJson(this);
    }
    public void RestoreData()
    {
        if (!string.IsNullOrEmpty(_backupJson))
        {
            JsonUtility.FromJsonOverwrite(_backupJson, this);
        }
    }
}
