using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestBonus
{
    public int experience;
    public int gold;
    [SerializeReference, ListDrawerSettings(ShowFoldout = true)]
    public List<QuestItem> itemsReward;
}
