using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent (typeof(DragDropSkill))]
public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Image m_IconImage;
    //[SerializeField] private Slider m_Slider;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InputAction m_ButtonPress;
    [SerializeField] private TextMeshProUGUI m_DeprisionSkill;
    [SerializeField] private Image m_SkillMarterial;
    [SerializeField] private Config configSO;
    //[SerializeField] private CheckButtonPress m_BoxButtonPress;
    private QuestItemSO m_CurrentItem;
    public QuestItemSO CurrentItem
    {
        get => m_CurrentItem;
        set
        {
            m_CurrentItem = value;
            if(m_CurrentItem != null)
            {
                if(ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UI_SEND_BUTTON_PRESS_AND_TYPESKILL,(m_ButtonPress,m_CurrentItem));
                }
            }
        }
    } 
        
    private Material m_Material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private void Start()
    {
        m_Material = configSO.skillBoxMaterial;
        m_DeprisionSkill = GetComponentInChildren<TextMeshProUGUI>();
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PU_BOXSKILL_SEND_TEXT, ReceiverEventPUBookSkillText);
        }
    }
    public void SetCurrenItem(QuestItemSO item)
    {
        m_CurrentItem = item;
    }
    public void SetSpriteSkillSLot(QuestItemSO questItemSO)
    {
        SetCurrenItem(questItemSO);
        m_IconImage.sprite = questItemSO.questItemData.icon;
        SetAlphaColor(1.0f);
    }
    public void SetMaterialSkillSlot()
    {
        m_SkillMarterial.material = m_Material;
        Color a = new(1,1,1,1);
        m_SkillMarterial.color = a;
    }
    public void HideMartiralSlot()
    {
        m_SkillMarterial.material = null ;
        Color a = new(1, 1, 1, 0);
        m_SkillMarterial.color = a;
    }    

    public void SetDeprisionSkill()
    {
        if (m_CurrentItem == null && m_DeprisionSkill == null) return;
        m_DeprisionSkill.text = m_CurrentItem.questItemData.DespristionSkill;
        
    }
    public void ClearItem()
    {
        m_IconImage.sprite = null;
        SetAlphaColor(0f);
        //m_IsEmpty = true;
    }
    public void SetAlphaColor(float alpha)
    {
        if (m_IconImage == null) return;

        Color temp = m_IconImage.color;
        temp.a = alpha;
        m_IconImage.color = temp;
    }
    private void ReceiverEventPUBookSkillText(object value)
    {
        if(value is TextMeshProUGUI text)
        {
            m_DeprisionSkill = text;
        }
    }
}
