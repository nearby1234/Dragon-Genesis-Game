using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogManager : BaseManager<DialogManager>
{

    [ShowInInspector]
    private readonly Dictionary<DialogState, List<DialogEntry>> dialogMap = new();
    [ShowInInspector]
    [InlineEditor]
    private DialogSystemSO currentDialogSO;
    public DialogSystemSO CurrentDialogSO => currentDialogSO;


    private QuestData currentQuestData;
    [SerializeField] private bool isDialogPlaying;
    public bool IsPlaying => isDialogPlaying;
    [SerializeField] private DialogState currentDialogState;
    //public DialogState CurrentDialogState
    //{
    //    get => currentDialogState;
    //}
    //private bool isDialogPlaying;

    public void StartDialog(DialogSystemSO dialogSO, QuestData questData = null)
    {
        if (isDialogPlaying) return;
        currentDialogSO = dialogSO;
        dialogMap.Clear();

        foreach (var entry in dialogSO.dialogEntries)
        {
            if (!dialogMap.ContainsKey(entry.state))
                dialogMap[entry.state] = new();
            dialogMap[entry.state].Add(entry);
        }
        // bắt đầu mặc định với state = Default
        ShowNextDialog(DialogState.Default);
        isDialogPlaying = true;
    }
    public void ShowNextDialog(DialogState state)
    {
        currentDialogState = state;

        if (dialogMap.TryGetValue(currentDialogState, out var listEntry) && listEntry.Count > 0)
        {
            var entry = listEntry[0];
            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowPopup<PopupDialogMission>(entry, true);
            }
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.SHOW_DIALOG_LINE, entry);
            }
            if (AudioManager.HasInstance)
            {
                if(entry.voiceClip != null)
                {
                    AudioManager.Instance.PlayVoiceSe(entry.voiceClip.name);
                }
               
            }
        }
        //else
        //{
        //    EndDialog();
        //}

    }
    public void EndDialog()
    {
        isDialogPlaying = false;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.END_DIALOG, null);
        }
    }

}
