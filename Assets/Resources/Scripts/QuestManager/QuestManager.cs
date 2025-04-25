using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using System.Linq;

public class QuestManager : BaseManager<QuestManager>
{
    [InlineEditor]
    [SerializeField] private QuestData currentQuest;
    [SerializeField] private SpawnObjectVFX _SliderPos;
    public int m_CountNumber;
    public QuestData CurrentQuest
    {
        get => currentQuest;
        set => currentQuest = value;
    }
    [InlineEditor]
    public List<QuestData> questList;
    public string m_QuestItemPrefabPath = "Prefabs/UI/Quest/ItemMission/ItemMissionImg";
    public string m_QuestRewardItemPrefabPath = "Prefabs/UI/Quest/ItemReward/ItemRewardImg";
    public string m_DOItemPrefabPath = "Prefabs/Inventory/DoSpite/DoSpite";

    // Các hằng số tween được định nghĩa lại ở đây để sử dụng trong GrantReward
    private readonly Vector2 TWEEN_TARGET_POS = new(-400f, -490f);
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

        questList = DataManager.Instance.GetAllData<QuestData,QuestType>();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CREEP_IS_DEAD,OnEnemyDead);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CREEP_IS_DEAD, OnEnemyDead);
        }
    }
    public void AcceptQuest(QuestData quest = null)
    {
        if (quest != null)
        {
            currentQuest = quest;
            quest.isAcceptMission = true;
            //if (questList.Contains(quest))
            //{
            //    return;
            //}
            //questList.Add(quest);
            Debug.Log("Nhận nhiệm vụ: " + currentQuest.questName);
            //UIManager.Instance.ShowScreen<QuestScreen>(currentQuest);
        }
        else
        {
           NextQuest();
            Debug.Log("Nhận nhiệm vụ: " + currentQuest.questName);
        }
    }

    public void NextQuest()
    {
        if (!currentQuest.isCompleteMission)
        {
            Debug.LogWarning($"Nhiệm vụ {currentQuest.questName} chưa hoàn thành.");
            return;
        }
        int index = questList.FindIndex(q => q.questID == currentQuest.questID);
        
        if(index < 0)
        {
            Debug.LogWarning($"Không tìm thấy nhiệm vụ hiện tại (ID: {currentQuest.questID}) trong questList.");
            return;
        }
        if (index +1 >= questList.Count)
        {
            Debug.LogWarning("Đã hoàn thành tất cả nhiệm vụ, không có nhiệm vụ tiếp theo.");
            return;
        }
        currentQuest = questList[index +1];
        Debug.Log($"Chuyển sang nhiệm vụ mới: {currentQuest.questName}");
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
        Transform rewardParent = UIManager.Instance.cPopup.transform;
        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab for reward items.");
            return;
        }

        foreach (var item in reward.itemsReward)
        {
            GameObject itemObj = Instantiate(prefab, rewardParent);
            Vector2 sliderPos = _SliderPos.TranslatePosition();
            if (itemObj.TryGetComponent<Image>(out Image image))
            {
                image.sprite = item.questItemData.icon;
            }

            if (itemObj.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            {
                if (item.questItemData.typeItem == TYPEITEM.ITEM_EXP)
                {
                    rectTransform.DOAnchorPos(sliderPos, TWEEN_DURATION)
                      .SetEase(Ease.InBack)
                      .OnComplete(() =>
                      {
                          if (PlayerLevelManager.HasInstance)
                          {
                              PlayerLevelManager.Instance.AddExp(item.questItemData.CountExp);
                          }
                          if (UIManager.HasInstance)
                          {
                              UIManager.Instance.SpawnObjectVFXPrefab.PlayAnimationFade();
                          }
                          Destroy(itemObj);
                      });
                }
                else
                {
                    rectTransform.DOAnchorPos(TWEEN_TARGET_POS, TWEEN_DURATION)
                        .SetEase(Ease.InBack)
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
        if (currentQuest != null && currentQuest.isCompleteMission)
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
    private void OnEnemyDead(object value)
    {
        if (value is not CreepType deadType) return;
        if (currentQuest == null) return;

        var first = currentQuest.ItemMission.FirstOrDefault(x=>x.questItemData.creepType == deadType);
        //Chỉ tăng khi đúng loại quest đang yêu cầu
        if (first !=null)
        {
            m_CountNumber++;
            first.questItemData.completionCount = m_CountNumber;
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.UI_UPDATE_ITEM_MISSION);
            }
            if (m_CountNumber.Equals(first.questItemData.requestCount))
            {
               currentQuest.isCompleteMission = true;
               if(ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.QUEST_COMPLETE, currentQuest.isCompleteMission);
                }
            }
        }
    }

    private void OnDisable()
    {
        // Khi thoát play mode, reset các số lượng cho các item reward
        ResetItemCounts();
    }
}
