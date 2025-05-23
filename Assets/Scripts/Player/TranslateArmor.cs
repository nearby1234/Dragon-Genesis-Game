using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TranslateArmor : MonoBehaviour
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
        }
    }

    private void TransLateArmorEquip(object value)
    {
        if(value is QuestItemSO currentitem)
        {
            m_CurrentItem = currentitem;
            TransferSkinnedMeshes(m_CurrentItem,false);
        } 
    }    

    private void TransferSkinnedMeshes(QuestItemSO currentitem , bool isShow)
    {
        // BƯỚC 1: Tìm armor cũ (trong listSkinnedBase) có cấu trúc bone giống armor mới
        ArmorIdentifier oldArmor = FindMatchingSkinnedBase(currentitem);
        var slotType = currentitem.questItemData.typeArmor;
        if(isShow)
        {
            if(equippedArmorDict.ContainsKey(slotType))
            {
                Destroy(equippedArmorDict[slotType]);
                equippedArmorDict.Remove(slotType);
            }    
            if(oldArmor != null)
            {
                oldArmor.gameObject.SetActive(true);
            }
            return;
        }
        //if (oldArmor != null && !isShow)
        //{
        //    oldArmor.gameObject.SetActive(isShow);
        //    Debug.Log("Đã ẩn armor cũ: " + oldArmor.name);
        //}else if(oldArmor != null && isShow)
        //{
        //    oldArmor.gameObject.SetActive(isShow);
        //    Debug.Log("Đã hiện armor cũ: " + oldArmor.name);
        //}
        //
        if (equippedArmorDict.ContainsKey(slotType))
        {
            Destroy(equippedArmorDict[slotType]);
            equippedArmorDict.Remove(slotType);
            Debug.Log("Đã xóa armor cũ ở slot: " + slotType);
        }
        // 2. Ẩn armor mặc định (nếu có)
        if (oldArmor != null)
        {
            oldArmor.gameObject.SetActive(false);
            Debug.Log("Đã ẩn armor base: " + oldArmor.name);
        }

        string cachedRootBoneName = currentitem.questItemData.skinnedArmor.rootBone.name;
        var newBones = new Transform[currentitem.questItemData.skinnedArmor.bones.Length];
        for(int i = 0;i< currentitem.questItemData.skinnedArmor.bones.Length;i++)
        {
            foreach(var newBone in m_NewArmature.GetComponentsInChildren<Transform>())
            {
                if(newBone.name == currentitem.questItemData.skinnedArmor.bones[i].name)
                {
                    newBones[i] = newBone;
                    break;
                }    
            }    
        }
        Transform matchingRootBone = GetRootBoneByName(m_NewArmature, cachedRootBoneName);
        currentitem.questItemData.skinnedArmor.rootBone = matchingRootBone != null ? matchingRootBone : m_NewArmature.transform;
        currentitem.questItemData.skinnedArmor.bones = newBones;
        Transform transform;
        //(transform = currentitem.questItemData.skinnedArmor.transform).SetParent(m_ParentArmor);
        transform = Instantiate(currentitem.questItemData.skinnedArmor.transform, m_ParentArmor);
        transform.localPosition = Vector3.zero;
        transform.gameObject.layer = config.layerIndex;
        equippedArmorDict[slotType] = transform.gameObject;
        Debug.Log($"Trang bị armor mới: {currentitem.questItemData.itemName} vào slot {slotType}");
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
        if(value is QuestItemSO itemSO)
        {
            Debug.Log($"itemSO : {itemSO.questItemData.itemName}");
            TransferSkinnedMeshes(itemSO, true);
        }
    }
}
