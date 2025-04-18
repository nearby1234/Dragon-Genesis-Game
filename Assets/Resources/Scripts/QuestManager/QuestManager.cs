using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class QuestManager : BaseManager<QuestManager>
{
    [InlineEditor]
    [SerializeField] private QuestData currentQuest;
    public QuestData CurrentQuest => currentQuest;
    [InlineEditor]
    public List<QuestData> questList;
    public string m_QuestItemPrefabPath = "Prefabs/UI/Quest/ItemMission/ItemMissionImg";
    public string m_QuestRewardItemPrefabPath = "Prefabs/UI/Quest/ItemReward/ItemRewardImg";
    public string m_DOItemPrefabPath = "Prefabs/Inventory/DoSpite/DoSpite";

    // Các hằng số tween được định nghĩa lại ở đây để sử dụng trong GrantReward
    private readonly Vector2 TWEEN_TARGET_POS = new Vector2(-400f, -490f);
    private const float TWEEN_DURATION = 2f;

    private Dictionary<string, int> initialItemCounts = new();

    protected override void Awake()
    {
        base.Awake();
        //BackupInitialCountItems();
    }

    private void Start()
    {
        foreach (var quest in questList)
        {
            foreach (var item in quest.ItemMission)
            {
                item.questItemData.completionCount = 0;
            }
        }
        // Backup số lượng ban đầu cho các item reward của currentQuest        
    }

    public void AcceptQuest(QuestData quest)
    {
        if (quest != null)
        {
            currentQuest = quest;
            quest.isAcceptMission = true;
            if (questList.Contains(quest))
            {
                return;
            }
            questList.Add(quest);
            Debug.Log("Nhận nhiệm vụ: " + currentQuest.questName);
            //UIManager.Instance.ShowScreen<QuestScreen>(currentQuest);
        }
        else
        {
            Debug.LogWarning("Chưa gán QuestData cho nhiệm vụ.");
        }
    }

    public void NextQuest()
    {
        if (currentQuest.isCompleteMission)
        {
            for (int i = 0; i < questList.Count; i++)
            {
                if (questList[i].questID == currentQuest.questID)
                {
                    currentQuest = questList[i + 1];
                    if(currentQuest == null)
                    {
                        Debug.LogWarning("Không tìm thấy nhiệm vụ tiếp theo.");
                        return;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Nhiệm vụ {currentQuest.name} chưa hoàn thành.");
        }
    }

    /// <summary>
    /// Phương thức GrantReward hiện đã chuyển toàn bộ logic của GetSpriteItemReward từ PopupScrollMagic.
    /// Nó chịu trách nhiệm hiển thị tween animation cho từng phần thưởng, cập nhật trạng thái quest,
    /// hiển thị các màn hình phụ (nếu cần) và phát sự kiện broadcast.
    /// </summary>
    /// <param name="reward">Đối tượng QuestBonus chứa thông tin phần thưởng</param>
    public void GrantReward(QuestBonus reward)
    {

        // Load prefab của phần thưởng
        GameObject prefab = Resources.Load<GameObject>(m_DOItemPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab for reward items.");
            return;
        }

        foreach (var item in reward.itemsReward)
        {
            // Ở đây, ta khởi tạo object phần thưởng dưới Transform của UIManager (nếu có) để đảm bảo hiển thị đúng UI

            Transform rewardParent;
            if (UIManager.Instance.CurPopup != null)
            {
                rewardParent = UIManager.Instance.CurPopup.transform;
            }
            else
            {
                rewardParent = UIManager.Instance.cPopup.transform;
            }
            GameObject itemObj = Instantiate(prefab, rewardParent);

            if (itemObj.TryGetComponent<Image>(out Image image))
            {
                image.sprite = item.questItemData.icon;
            }

            if (itemObj.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            {
                rectTransform.DOAnchorPos(TWEEN_TARGET_POS, TWEEN_DURATION)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (CurrentQuest != null && CurrentQuest.questID == "-QuestMissionOne" && UIManager.HasInstance)
                        {
                            UIManager.Instance.ShowScreen<ScreenIconInventory>();
                            UIManager.Instance.ShowScreen<ScreenBox>();
                        }
                        Destroy(itemObj);
                    });
            }
        }

        // Cập nhật trạng thái mission đã hoàn thành của quest hiện tại
        if (CurrentQuest != null)
        {
            CurrentQuest.isCompleteMission = true;
        }

        // Gửi sự kiện về phần thưởng đã được cấp
        ListenerManager.Instance?.BroadCast(ListenType.UI_SEND_LIST_ITEM_REWARD, reward.itemsReward);
    }

    public void CompleteQuest()
    {
        if (currentQuest != null)
        {
            Debug.Log("Hoàn thành nhiệm vụ: " + currentQuest.questName);
            // Cấp phần thưởng cho người chơi thông qua GrantReward
            GrantReward(currentQuest.bonus);
            // Hiển thị thông báo hoàn thành nhiệm vụ qua UIManager
            //UIManager.Instance.ShowNotify<QuestNotify>(currentQuest);
        }
    }

    private void ResetItemCounts()
    {
        if (currentQuest == null)
        {
            Debug.LogWarning("Không tìm thấy current quest để reset item count.");
            return;
        }
        foreach (var item in currentQuest.bonus.itemsReward)
        {
            string key = item.questItemData.itemName;
            if (initialItemCounts.ContainsKey(key))
            {
                item.questItemData.count = initialItemCounts[key];
            }
        }
    }

    private void OnDisable()
    {
        // Khi thoát play mode, reset các số lượng cho các item reward
        ResetItemCounts();
    }
}
