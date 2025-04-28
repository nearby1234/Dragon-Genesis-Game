using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class HandleCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<RectTransform> items;
    private void Start()
    {
        AddItemMission();
    }
    public void HideCanvasGroup()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void ShowCanvasGroup()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(1f, 1f);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public void UpdateTextItemMission(QuestItemSO questItemSO)
    {
        if (questItemSO == null) return;
        if (!questItemSO.questItemData.typeItem.Equals(TYPEITEM.ITEM_COLLECT)) return;
        foreach (var item in this.items)
        {
            TextMeshProUGUI textMeshProUGUI = item.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshProUGUI != null)
                textMeshProUGUI.text = $"{questItemSO.questItemData.itemName} {questItemSO.questItemData.completionCount}/{questItemSO.questItemData.requestCount}";
            else
                Debug.LogWarning($"not found {item.name} textMeshProUGUI");
        }
    }
    public void AddItemMission()
    {
        // 1. Clear cache c?
        items.Clear();

        // 2. Thêm l?i t? transform.childCount
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<RectTransform>(out var rect))
                items.Add(rect);
        }
    }
    public void ClearItemsList()
    {
        //items.RemoveAll(item => item == null);
        items.Clear();
    }
}
