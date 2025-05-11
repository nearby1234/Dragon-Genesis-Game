using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupBoxSkill : BasePopup
{
    [SerializeField] private GameObject m_ParentBoxSkill;
    [SerializeField] private GameObject m_ParentSkillSlotPanel;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private List<GameObject> boxPrefabsList = new();
    [SerializeField] private List<GameObject> skillSLotList = new();
    [SerializeField] private int m_CountBox = 0;
    [SerializeField] private int m_CountSkillSlot = 0;
    [SerializeField] private Config configSO;
    public GameObject m_OriginalClickSkillSlot;
    public GameObject m_CurrentClickSkillSlot;


    private GameObject m_BoxSkillPrefabs;
    private string boxSkillPrefabs_PATH;
    private GameObject m_SkillSLotPrefabs;
    private string skillSLotPrefabs_PATH;

    private void Awake()
    {
        boxSkillPrefabs_PATH = configSO.boxSkillPrefabs_PATH;
        skillSLotPrefabs_PATH = configSO.skillSlotPrefabs_PATH;
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.UI_SEND_LIST_ITEM_REWARD, ReceiverEventSkillSlot);
            ListenerManager.Instance.BroadCast(ListenType.PU_BOXSKILL_SEND_TEXT, descriptionTxt);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_LIST_ITEM_REWARD, ReceiverEventSkillSlot);
        }
    }
    private void FillerItemSKill(List<QuestItemSO> questItemSO)
    {
        for (int i = 0; i < questItemSO.Count; i++)
        {
            if (questItemSO[i].questItemData.typeItem.Equals(TYPEITEM.ITEM_SKILL))
            {
                InitBoxSkillSLot(questItemSO[i]);
                Debug.Log($"questItemSkill : {questItemSO[i].name}");
            }
        }
    }

    private void InitBoxSkillSLot(QuestItemSO questItemSO)
    {
        m_BoxSkillPrefabs = Resources.Load<GameObject>(boxSkillPrefabs_PATH);
        m_SkillSLotPrefabs = Resources.Load<GameObject>(skillSLotPrefabs_PATH);
        GameObject boxPrefabs = Instantiate(m_BoxSkillPrefabs, m_ParentBoxSkill.transform);
        m_CountBox++;
        boxPrefabs.name = $"Box Skill {m_CountBox}";
        boxPrefabsList.Add(boxPrefabs);
        GameObject SkillSlotPrefabs = Instantiate(m_SkillSLotPrefabs, m_ParentSkillSlotPanel.transform);
        m_CountSkillSlot++;
        SkillSlotPrefabs.name = $"Skill Slot Obj{m_CountSkillSlot}";
        InitSKillSLot(SkillSlotPrefabs, questItemSO);
        skillSLotList.Add(SkillSlotPrefabs);
    }

    private void InitSKillSLot(GameObject gameObject, QuestItemSO questItemSO)
    {
        SkillSlot skillSlot = gameObject.GetComponentInChildren<SkillSlot>();
        if (skillSlot != null)
        {
            skillSlot.SetSpriteSkillSLot(questItemSO);
        }
    }

    private void ReceiverEventSkillSlot(object value)
    {
        if (value is List<QuestItemSO> itemSO)
        {
            FillerItemSKill(itemSO);
        }
    }
}
