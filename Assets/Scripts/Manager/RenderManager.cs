using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager : BaseManager<RenderManager>
{
    [SerializeField] private GameObject m_ParentWeapon;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private GameObject m_WeaponPrefabs;
    [SerializeField] private Config config;
    [SerializeField] private RotateModelPlayer m_RotateModelPlayer;
    public RotateModelPlayer RotateModelPlayer => m_RotateModelPlayer;
    [ShowInInspector]
    private Dictionary<string, GameObject> m_Items = new();

    protected override void Awake()
    {
        base.Awake();
        m_RotateModelPlayer = GetComponent<RotateModelPlayer>();
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
            SetParentWeaponUI(m_CurrentItem);
        }
    }
    private void SetParentWeaponUI(QuestItemSO itemSO)
    {
        var raw = itemSO.questItemData.m_SwordPrefabs;
        if (raw == null)
        {
            Debug.LogError("m_SwordPrefabs == null!");
            return;
        }
        Debug.Log($"Type của m_SwordPrefabs là: {raw.GetType().Name}");
        if (!(raw is GameObject))
        {
            Debug.LogError($"Expected GameObject nhưng m_SwordPrefabs là {raw.GetType().Name}");
            return;
        }
        var goPrefab = raw as GameObject;
        if (itemSO == null) return;
        m_WeaponPrefabs = itemSO.questItemData.m_SwordPrefabs;
        GameObject weaponPrefabs = Instantiate(goPrefab, m_ParentWeapon.transform);
        weaponPrefabs.layer = config.layerIndex;
        m_Items.Add(itemSO.questItemData.itemID, weaponPrefabs);
    }
    private void OnEventHideWeapon(object value)
    {
        if (value is QuestItemSO itemSO)
        {
            if (m_Items.TryGetValue(itemSO.questItemData.itemID, out var weaponObject))
            {
                Destroy(weaponObject);
                m_Items.Remove(itemSO.questItemData.itemID);
            }
        }
    }

}
