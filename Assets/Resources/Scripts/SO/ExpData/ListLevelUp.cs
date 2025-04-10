using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelUp", menuName = "Scriptable Object/Exp/ListLevelUp")]
[System.Serializable]
public class ListLevelUp : ScriptableObject
{
    public int level;
    public int CurrentExp;
    public int expNeedLvup;
    public int totalExp;
}
