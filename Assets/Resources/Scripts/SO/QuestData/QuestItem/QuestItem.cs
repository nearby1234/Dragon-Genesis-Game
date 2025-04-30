using Sirenix.OdinInspector;
using UnityEngine;

public enum TYPEITEM
{
    DEFAULT = 0,
    ITEM_MISSION,
    ITEM_USE,
    ITEM_EQUIP,
    ITEM_COLLECT,
    ITEM_EXP,
    ITEM_SKILL,
    ITEM_WEAPON,
    ITEM_ARMOR,
}

public enum TYPEWEAPON
{
    DEFAULT = 0,
    WEAPON_WHITE,
    WEAPON_BLUE,
}
public enum TYPEARMOR
{
    DEFAULT = 0,
    ARMOR_CHEST,
    ARMOR_HEAD,
    ARMOR_BELT,
    ARMOR_BOOTS,
    ARMOR_GLOVES,
    ARMOR_ARMS,
    ARMOR_LEGS,
    ARMOR_SHOULDERS,
   
}
public enum ITEMUSE
{
    DEFAULT = 0,
    ITEM_USE_HEAL,
    ITEM_USE_MANA,
    ITEM_USE_BUFF,
    ITEM_USE_DEBUFF,
}

[System.Serializable]
public class QuestItem
{
    public string itemID;
    public string itemName;
    public int count;
    public int requestCount;
    public int completionCount;
    [PreviewField(70, ObjectFieldAlignment.Left)]
    public Sprite icon;
    public TYPEITEM typeItem;

    // Chỉ hiện khi là ITEM_USE
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public float percentIncrease;
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public float timeCoolDown;
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    public ITEMUSE itemUse;
    [ShowIf("@typeItem == TYPEITEM.ITEM_USE")]
    [BoxGroup("Item Use Stats")]
    [TextArea(3, 10)]
    public string ItemDespri;

    [ShowIf("@typeItem == TYPEITEM.ITEM_EXP")]
    [BoxGroup("Item Use Stats")]
    public int CountExp;

    [ShowIf("@typeItem == TYPEITEM.ITEM_COLLECT")]
    [BoxGroup("Item Use Stats")]
    public CreepType creepType;

    [ShowIf("@typeItem == TYPEITEM.ITEM_SKILL")]
    [BoxGroup("Item Use Stats")]
    public float TimerSkillCoolDown;
    [ShowIf("@typeItem == TYPEITEM.ITEM_SKILL")]
    [BoxGroup("Item Use Stats")]
    [TextArea(3, 10)]
    public string DespristionSkill;

    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public TYPEWEAPON typeWeapon;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public GameObject m_SwordPrefabs;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public Mesh m_SwordMesh;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public Material m_SwordMaterial;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    [TextArea(3, 10)]
    public string DespristionWeapon;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public int plusStrength;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public int plusAgility;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public int plusStamina;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public int plusHeal;
    [ShowIf("@typeItem == TYPEITEM.ITEM_WEAPON")]
    [BoxGroup("Item Use Stats")]
    public int plusAmor;

    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public TYPEARMOR typeArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public SkinnedMeshRenderer skinnedArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    [TextArea(3, 10)]
    public string DespristionArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public int plusStrengthArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public int plusAgilityArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public int plusStaminaArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public int plusHealArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public int plusDefendArmor;
    [ShowIf("@typeItem == TYPEITEM.ITEM_ARMOR")]
    [BoxGroup("Item Use Stats")]
    public string m_NameArmorPrefabs;


}
