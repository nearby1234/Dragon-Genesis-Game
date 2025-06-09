using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject m_ParentWeapon;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private GameObject m_CurrentWeapon;
    [SerializeField] private MeshFilter m_CurrentWeaponMesh;
    [SerializeField] private MeshRenderer m_CurrentWeaponMeshRender;
    [SerializeField] private MeshFilter m_EnergyMesh;
    public QuestItemSO CurrentItem => m_CurrentItem;
    //[SerializeField] private Config config;
    [ShowInInspector]
    private Dictionary<string, GameObject> m_Items = new();
    private void Awake()
    {
        m_CurrentWeaponMesh = m_CurrentWeapon.GetComponent<MeshFilter>();
        m_CurrentWeaponMeshRender = m_CurrentWeapon.GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_WEAPON_UI, ReceiverEventShowPlayerWeaponUI);
            ListenerManager.Instance.Register(ListenType.HIDE_ITEM_WEAPON_UI, OnEventHideWeapon);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_WEAPON_UI, ReceiverEventShowPlayerWeaponUI);
            ListenerManager.Instance.Unregister(ListenType.HIDE_ITEM_WEAPON_UI, OnEventHideWeapon);
        }
    }

    private void ReceiverEventShowPlayerWeaponUI(object value)
    {
        if (value is QuestItemSO itemSO)
        {
            m_CurrentItem = itemSO;

            BroadcastAllStatDeltas(itemSO, true);
            SetWeapon(m_CurrentItem);
        }
    }
    private void OnEventHideWeapon(object value)
    {
        if (value is QuestItemSO itemSO)
        {
            if(m_CurrentItem.questItemData.m_SwordMesh == itemSO.questItemData.m_SwordMesh)
            {
                m_CurrentItem = null;
                if(m_CurrentItem == null)
                {
                    if(ListenerManager.HasInstance)
                    {
                        ListenerManager.Instance.BroadCast(ListenType.PLAYER_NOT_WEAPON, null);
                    }
                }
                if (m_CurrentWeaponMesh != null) m_CurrentWeaponMesh.mesh = null;
                if (m_CurrentWeaponMeshRender != null) m_CurrentWeaponMeshRender.material = null;
                if (m_EnergyMesh != null) m_EnergyMesh.mesh = null;
                BroadcastAllStatDeltas(itemSO, false);
            }
        }
    }
    private void SetWeapon(QuestItemSO itemSO)
    {
        if (itemSO == null) return;
        if(m_CurrentWeaponMesh != null) m_CurrentWeaponMesh.mesh = m_CurrentItem.questItemData.m_SwordMesh;
        if(m_CurrentWeaponMeshRender != null) m_CurrentWeaponMeshRender.material = m_CurrentItem.questItemData.m_SwordMaterial;
        if (m_EnergyMesh != null) m_EnergyMesh.mesh = m_CurrentItem.questItemData.m_SwordMesh;
        Debug.Log($"m_SwordMesh : {m_CurrentItem.questItemData.m_SwordMesh}"  );

    }
    private void BroadcastAllStatDeltas(QuestItemSO itemSO, bool isEquip)
    {
        // 1) Tạo dict tĩnh hoặc mỗi lần build lại từ questItemData
        var deltas = new Dictionary<TYPESTAT, int>()
    {
        { TYPESTAT.STR, itemSO.questItemData.plusStrengthArmor },
        { TYPESTAT.ITE, itemSO.questItemData.plusAgilityArmor },
        { TYPESTAT.HEA, itemSO.questItemData.plusHealArmor },
        { TYPESTAT.DEF, itemSO.questItemData.plusDefendArmor },
        { TYPESTAT.STA, itemSO.questItemData.plusStaminaArmor },
        // ... thêm các stat khác nếu có
    };

        // 2) Duyệt và gửi event
        foreach (var kvp in deltas)
        {
            if (kvp.Value == 0)
                continue;   // bỏ qua stat không thay đổi

            var delta = isEquip ? kvp.Value : -kvp.Value;
            var payload = new StatEquipData
            {
                StatType = kvp.Key,
                ValueDelta = delta
            };
            ListenerManager.Instance.BroadCast(
                isEquip
                  ? ListenType.EQUIP_STAT_UPDATE
                  : ListenType.UNEQUIP_STAT_UPDATE,
                payload
            );
        }
    }
}
