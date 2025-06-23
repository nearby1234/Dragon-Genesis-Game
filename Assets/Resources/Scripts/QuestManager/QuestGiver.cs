using System.Runtime.InteropServices;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    public QuestData QuestData => questData;
    [SerializeField] private DialogSystemSO dialogSystemSO;
    public DialogSystemSO DialogSystemSO => dialogSystemSO;
    public NPCName nPCName;
    public NPCName NPCName => nPCName;
    [SerializeField] private bool isSendQuestMission;
    public bool IsSendQuestMission
    {
        get => isSendQuestMission;
        set => isSendQuestMission = value;
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.END_DIALOG, OnEvenEndDialog);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.END_DIALOG, OnEvenEndDialog);
        }
    }
    private void OnEvenEndDialog(object value)
    {
        if(QuestManager.HasInstance)
        {
            questData = QuestManager.Instance.CurrentQuest;
            IsSendQuestMission = false;
        }
        
    }

}
