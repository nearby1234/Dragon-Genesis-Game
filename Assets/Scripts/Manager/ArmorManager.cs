using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmorManager : BaseManager<ArmorManager>
{
    [SerializeField] private Transform m_ParentTranform;
    [SerializeField] private List<SkinnedMeshRenderer> skinnedMeshesMinotaurList = new();
    [ShowInInspector]
    private Dictionary<string,SkinnedMeshRenderer> skinnedMeshesMinotaurDict = new();
    [InlineEditor]
    [SerializeField] private List<QuestItemSO> questItemSOList = new();

    private void Start()
    {
        for(int i = 0;i<skinnedMeshesMinotaurList.Count;i++)
        {
            SkinnedMeshRenderer meshRenderer = skinnedMeshesMinotaurList[i];
            skinnedMeshesMinotaurDict[meshRenderer.name] = meshRenderer;
        }    
        for(int i = 0; i<questItemSOList.Count; i++)
        {
            QuestItemSO questItem = questItemSOList[i];
            // Kiểm tra xem tên trong questItem có tồn tại trong dictionary không
            if (skinnedMeshesMinotaurDict.TryGetValue(questItem.questItemData.m_NameArmorPrefabs, out SkinnedMeshRenderer foundMesh))
            {
                // Gán SkinnedMeshRenderer vào QuestItemSO
                SkinnedMeshRenderer skinnedMeshRenderer = Instantiate(foundMesh,m_ParentTranform.transform);
                questItem.questItemData.skinnedArmor = skinnedMeshRenderer;
            }
            else
            {
                Debug.LogWarning($"No SkinnedMeshRenderer found for {questItem.name}");
            }
        }    

    }
}
