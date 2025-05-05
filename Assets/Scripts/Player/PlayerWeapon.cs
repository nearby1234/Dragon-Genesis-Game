using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject m_ParentWeapon;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private GameObject m_CurrentWeapon;
    [SerializeField] private MeshFilter m_CurrentWeaponMesh;
    [SerializeField] private MeshRenderer m_CurrentWeaponMeshRender;
    [SerializeField] private MeshFilter m_EnergyMesh;
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
        if (value is QuestItemSO itemSO)
        {
            m_CurrentItem = itemSO;
            SetWeapon(m_CurrentItem);
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
}
