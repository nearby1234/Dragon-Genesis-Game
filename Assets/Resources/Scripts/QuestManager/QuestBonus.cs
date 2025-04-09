using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestBonus
{
    public int experience;
    public int gold;
    [InlineEditor]
    public List<QuestItemSO> itemsReward;
}
