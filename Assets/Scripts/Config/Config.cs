using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Config", menuName = "Config")]
public class Config : ScriptableObject
{
    [Header("Link Path Quest")]
    public string m_QuestItemPrefabPath = "Prefabs/UI/Quest/ItemMission/ItemMissionImg";
    public string m_QuestRewardItemPrefabPath = "Prefabs/UI/Quest/ItemReward/ItemRewardImg";
    public string m_DOItemPrefabPath = "Prefabs/UI/Inventory/DoSpite/DoSpite";

    [Header("Link Path Inventory")]
    public string m_BoxImgPath = "Prefabs/UI/Inventory/BoxItem/Box";
    public string m_ItemImgPath = "Prefabs/UI/Inventory/Item/ItemImg";

    [Header("Link Path SKillBox")]
    public string boxSkillPrefabs_PATH = "Prefabs/UI/BoxSkill/BoxSkill";
    public string skillSlotPrefabs_PATH = "Prefabs/UI/BoxSkill/SkillSlot";

    [Header("Material Skill Box")]
    public Material skillBoxMaterial;

    [Header("Name Questtions")]
    public  string NameQuestMissionOne = "-QuestMissionOne";
    public  string NameQuestMissionTwo = "-QuestMissionTwo";

    [Header("Layer Index Show PlayerUI")]
    public int layerIndex;

    [Header("Index Left Padding ToolTip")]
    public int paddingLeftItem;
    public int paddingLeftArmor;



}
