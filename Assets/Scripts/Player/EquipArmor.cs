using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipArmor : MonoBehaviour
{
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private Transform m_ParentArmor;
    [SerializeField] private Transform m_NewArmature;
    [SerializeField] private Config config;
    [SerializeField] private Transform m_ParentTranformBase;
    [SerializeField] private List<ArmorIdentifier> listSkinnedBase;
    [ShowInInspector]
    private Dictionary<TYPEARMOR, GameObject> equippedArmorDict = new();


    private void Start()
    {
        GetChildTranformSkinned();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_HEAD_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_CHEST_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_ARMS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_BELT_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_BOOTS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_GLOVES_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_LEGS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.SHOWPLAYER_ARMOR_SHOULDERS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Register(ListenType.HIDE_ITEM_ARMOR_UI, HideArmor);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_HEAD_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_CHEST_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_ARMS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_BELT_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_BOOTS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_GLOVES_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_LEGS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.SHOWPLAYER_ARMOR_SHOULDERS_UI, TransLateArmorEquip);
            ListenerManager.Instance.Unregister(ListenType.HIDE_ITEM_ARMOR_UI, HideArmor);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }

    private void TransLateArmorEquip(object value)
    {
        if (value is QuestItemSO currentitem)
        {
            m_CurrentItem = currentitem;
            TransferSkinnedMeshes(m_CurrentItem, false);
        }
    }

    private void TransferSkinnedMeshes(QuestItemSO currentItem, bool isShowBaseArmor)
    {
        var slotType = currentItem.questItemData.typeArmor;

        // 1) Nếu là yêu cầu gỡ (isShowBaseArmor = true) → hủy instance cũ và hiện base armor
        if (isShowBaseArmor)
        {
            BroadcastAllStatDeltas(currentItem,false);

            if (equippedArmorDict.TryGetValue(slotType, out var oldGo))
            {
                Destroy(oldGo);
                equippedArmorDict.Remove(slotType);
            }

            // hiện lại base armor ban đầu (nếu có)
            var baseArmor = FindMatchingSkinnedBase(currentItem);
            if (baseArmor != null)
                baseArmor.gameObject.SetActive(true);

            return;
        }
        BroadcastAllStatDeltas(currentItem, true);

        // 2) Nếu trang bị mới → trước hết hủy instance trong slot (nếu đã có)
        if (equippedArmorDict.TryGetValue(slotType, out var existGo))
        {
            Destroy(existGo);
            equippedArmorDict.Remove(slotType);
        }

        // 3) Ẩn base armor mặc định
        var oldBase = FindMatchingSkinnedBase(currentItem);
        if (oldBase != null)
            oldBase.gameObject.SetActive(false);

        // 4) Instantiate prefab gốc
        var prefab = currentItem.questItemData.skinnedArmor;
        if (prefab == null)
        {
            return;
        }

        SkinnedMeshRenderer newSkin = Instantiate(prefab, m_ParentArmor);
        newSkin.transform.localPosition = Vector3.zero;
        newSkin.gameObject.layer = config.layerIndex;

        // 5) Rebind bones sang armature mới
        string rootName = prefab.rootBone.name;
        var newBones = new Transform[prefab.bones.Length];
        for (int i = 0; i < prefab.bones.Length; i++)
        {
            var boneName = prefab.bones[i].name;
            newBones[i] = m_NewArmature.GetComponentsInChildren<Transform>()
                              .FirstOrDefault(t => t.name == boneName);
        }
        Transform newRoot = m_NewArmature.GetComponentsInChildren<Transform>()
                             .FirstOrDefault(t => t.name == rootName)
                         ?? m_NewArmature;
        newSkin.rootBone = newRoot;
        newSkin.bones = newBones;

        // 6) Lưu vào dict để quản lý slot này
        equippedArmorDict[slotType] = newSkin.gameObject;
        newSkin.gameObject.tag = "Player";
    }


    private Transform GetRootBoneByName(Transform parentTransform, string name)
    {
        return parentTransform.GetComponentsInChildren<Transform>().FirstOrDefault(transformChild => transformChild.name == name);
    }
    private ArmorIdentifier FindMatchingSkinnedBase(QuestItemSO currentitem)
    {
        //var newID = newArmor.GetComponent<ArmorIdentifier>();
        //if (newID == null) return null;

        foreach (var oldArmor in listSkinnedBase)
        {
            if (oldArmor == null) continue;

            if (!oldArmor.TryGetComponent<ArmorIdentifier>(out var oldID)) continue;

            if (oldID.typeArmor == currentitem.questItemData.typeArmor)
            {
                return oldArmor;
            }
        }

        return null;
    }

    private void GetChildTranformSkinned()
    {
        foreach (var child in m_ParentTranformBase.GetComponentsInChildren<ArmorIdentifier>())
        {
            if (child != null)
                listSkinnedBase.Add(child);
        }
    }

    private void HideArmor(object value)
    {
        if (value is QuestItemSO itemSO)
        {
            TransferSkinnedMeshes(itemSO, true);
        }
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
    private void ReceiverEventClickMainMenu(object value)
    {
        m_CurrentItem = null;
    }

}
