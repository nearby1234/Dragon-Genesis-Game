using System.Runtime.InteropServices;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    public QuestData QuestData => questData;
    [SerializeField] private DialogSystemSO dialogSystemSO;
    public DialogSystemSO DialogSystemSO => dialogSystemSO;
    [SerializeField] public NPCName nPCName;
    public NPCName NPCName => nPCName;
}
