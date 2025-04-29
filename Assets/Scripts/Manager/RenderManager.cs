using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager : BaseManager<RenderManager>
{
    [SerializeField] private GameObject m_ParentWeapon;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private GameObject m_WeaponPrefabs;
    [SerializeField] private Config config;
    [ShowInInspector]
    private Dictionary<string, GameObject> m_Items = new();
    

    private void Start()
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_WEAPON_UI, ReceiverEventShowPlayerWeaponUI);
        }    
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_WEAPON_UI, ReceiverEventShowPlayerWeaponUI);
        }
    }

    private void ReceiverEventShowPlayerWeaponUI(object value)
    {
        if(value is QuestItemSO itemSO)
        {
            m_CurrentItem = itemSO;
            SetParentWeaponUI(m_CurrentItem);
        }    
    }    
    private void SetParentWeaponUI(QuestItemSO itemSO)
    {
        if (itemSO == null) return;
        m_WeaponPrefabs = itemSO.questItemData.m_SwordPrefabs;
        GameObject weaponPrefabs = Instantiate(m_WeaponPrefabs,m_ParentWeapon.transform);
        weaponPrefabs.layer = config.layerIndex;
        m_Items.Add(itemSO.questItemData.itemID, weaponPrefabs);
    }    
}
