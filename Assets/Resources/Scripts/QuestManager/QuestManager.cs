using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;

public class QuestManager : BaseManager<QuestManager>
{
    [InlineEditor]
    [SerializeField] private QuestData currentQuest;
    [SerializeField] private SpawnObjectVFX _SliderPos;
    public Config configSO;

    [InlineEditor]
    [ListDrawerSettings(ShowPaging = false)]
    public List<QuestData> questList;
    public string m_QuestItemPrefabPath;
    public string m_QuestRewardItemPrefabPath;
    public string m_DOItemPrefabPath;
    public const string NameQuestMissionOne = "-QuestMission1";
    public const string NameQuestMissionTwo = "-QuestMission2";
    public const string NameQuestMissionThree = "-QuestMission3";
    public int m_CountNumber;

    // Các hằng số tween được định nghĩa lại ở đây để sử dụng trong GrantReward
    private readonly Vector2 Screen_IconBag_Pos = new(-200f, -535f);
    private readonly Vector2 Screen_IconBookSkill_Pos = new(0, -540f);
    private const float TWEEN_DURATION = 2f;
    public QuestData CurrentQuest
    {
        get => currentQuest;
        set => currentQuest = value;
    }

    private Dictionary<string, int> initialItemCounts = new();
    private void Start()
    {
        m_QuestItemPrefabPath = configSO.m_QuestItemPrefabPath;
        m_QuestRewardItemPrefabPath = configSO.m_QuestRewardItemPrefabPath;
        m_DOItemPrefabPath = configSO.m_DOItemPrefabPath;
        foreach (var quest in questList)
        {
            foreach (var item in quest.ItemMission)
            {
                item.questItemData.completionCount = 0;
            }
        }

        questList = DataManager.Instance.GetAllData<QuestData, QuestType>();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CREEP_IS_DEAD, OnEnemyDead);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CREEP_IS_DEAD, OnEnemyDead);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
        }
    }
    public void AcceptQuest(QuestData quest = null)
    {
        if (quest != null)
        {
            currentQuest = quest;
            quest.isAcceptMission = true;

            Debug.Log($"m_CountNumber accept : {m_CountNumber}");
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
            m_CountNumber = 0;
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

        if (index < 0)
        {
            Debug.LogWarning($"Không tìm thấy nhiệm vụ hiện tại (ID: {currentQuest.questID}) trong questList.");
            return;
        }
        if (index + 1 >= questList.Count)
        {
            Debug.LogWarning("Đã hoàn thành tất cả nhiệm vụ, không có nhiệm vụ tiếp theo.");
            return;
        }
        currentQuest = questList[index + 1];
        Debug.Log($"Chuyển sang nhiệm vụ mới: {currentQuest.questName}");
    }

    public void GrantReward(QuestBonus reward)
    {
        GameObject prefab = Resources.Load<GameObject>(m_DOItemPrefabPath);
        Transform rewardParent = UIManager.Instance.cPopup.transform;
        if (prefab == null)
        {
            Debug.LogWarning("Failed to load prefab for reward items.");
            return;
        }

        var clones = new List<QuestItemSO>(reward.itemsReward.Count);
        foreach (var item in reward.itemsReward)
        {
            var itemObj = Instantiate(prefab, rewardParent);
            var runtimeItem = Instantiate(item);
            clones.Add(runtimeItem);
            //var sliderPos = _SliderPos.TranslatePosition();

            // Set icon nếu có
            if (itemObj.TryGetComponent<Image>(out var image))
                image.sprite = item.questItemData.icon;

            if (itemObj.TryGetComponent<RectTransform>(out var rt))
            {
                // Chỉ gọi 1 dòng duy nhất, chuyển cho phương thức tương ứng
                HandleByType(runtimeItem, rt, itemObj);
            }

            // Đánh dấu hoàn thành quest + broadcast

        }
        if (CurrentQuest != null) CurrentQuest.isCompleteMission = true;
        ListenerManager.Instance?.BroadCast(ListenType.UI_SEND_LIST_ITEM_REWARD, clones);
    }
    private void HandleByType(QuestItemSO item, RectTransform rt, GameObject itemObj)
    {
        switch (item.questItemData.typeItem)
        {
            case TYPEITEM.ITEM_EXP:
                HandleExp(item, rt, _SliderPos.TranslatePosition(), itemObj);
                break;

            case TYPEITEM.ITEM_MISSION:
                HandleMission(item, rt, itemObj);
                break;

            case TYPEITEM.ITEM_USE:
                HandleUse(item, rt, Screen_IconBag_Pos, itemObj);
                break;

            case TYPEITEM.ITEM_SKILL:
                HandleSkill(item, rt, Screen_IconBookSkill_Pos, itemObj);
                break;

            case TYPEITEM.ITEM_WEAPON:
                HandleUse(item, rt, Screen_IconBag_Pos, itemObj);
                break;
            case TYPEITEM.ITEM_ARMOR:
                HandleUse(item, rt, Screen_IconBag_Pos, itemObj);
                break;
            // TODO: nếu sau này có thêm loại mới, chỉ cần thêm case ở đây
            default:
                Debug.LogWarning($"Unknown reward type: {item.questItemData.typeItem}");
                break;
        }
    }
    private void HandleExp(QuestItemSO item, RectTransform rt, Vector2 targetPos, GameObject itemObj)
    {
        rt.DOAnchorPos(targetPos, TWEEN_DURATION)
          .SetEase(Ease.InBack)
          .OnComplete(() =>
          {
              PlayerLevelManager.Instance?.AddExp(item.questItemData.CountExp);
              UIManager.Instance?.SpawnObjectVFXPrefab.PlayAnimationFade();
              Destroy(itemObj);
          });
    }
    private void HandleUse(QuestItemSO item, RectTransform rt, Vector2 targetPos, GameObject itemObj)
    {
        rt.DOAnchorPos(targetPos, TWEEN_DURATION)
          .SetEase(Ease.InBack)
          .OnComplete(() =>
          {
              SoundReward();
              Destroy(itemObj);
          });
    }
    private void HandleSkill(QuestItemSO item, RectTransform rt, Vector2 targetPos, GameObject itemObj)
    {
        rt.DOAnchorPos(targetPos, TWEEN_DURATION).SetEase(Ease.InBack).OnComplete(() =>
        {
            SoundReward();
            Destroy(itemObj);
        });
    }

    private void HandleMission(QuestItemSO item, RectTransform rt, GameObject itemObj)
    {
        // Nếu logic mission phụ thuộc vào questID, tách tiếp
        switch (currentQuest.questID)
        {
            case NameQuestMissionOne:
                rt.DOAnchorPos(Screen_IconBag_Pos, TWEEN_DURATION)
               .SetEase(Ease.InBack)
               .OnComplete(() =>
               {
                   if (UIManager.HasInstance)
                   {
                       UIManager.Instance.ShowScreen<ScreenIconInventory>();
                       UIManager.Instance.ShowScreen<ScreenBox>();
                   }
                   SoundReward();
                   Destroy(itemObj);
               });
                break;
            case NameQuestMissionTwo:
                rt.DOAnchorPos(Screen_IconBookSkill_Pos, TWEEN_DURATION)
               .SetEase(Ease.InBack)
               .OnComplete(() =>
              {
                  if (UIManager.HasInstance)
                  {
                      UIManager.Instance.ShowScreen<ScreenBoxSkill>();
                      UIManager.Instance.ShowScreen<ScreenBookSkill>();
                  }
                  SoundReward();
                  Destroy(itemObj);
              });
                break;
            default:
                Debug.LogWarning($"Unknown mission questID: {currentQuest.questID}");
                Destroy(itemObj);
                break;
        }
    }

    //private void PlayMissionTween(RectTransform rt, Vector2 destination, GameObject itemObj)
    //{
    //    rt.DOAnchorPos(destination, TWEEN_DURATION)
    //      .SetEase(Ease.InBack)
    //      .OnComplete(() =>
    //      {
    //          UIManager.Instance.ShowScreen<ScreenIconInventory>();
    //          UIManager.Instance.ShowScreen<ScreenBox>();
    //          Destroy(itemObj);
    //      });
    //}

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

        var first = currentQuest.ItemMission.FirstOrDefault(x => x.questItemData.creepType == deadType);
        //Chỉ tăng khi đúng loại quest đang yêu cầu
        if (first != null)
        {
            m_CountNumber++;
            Debug.Log($"m_CountNumber : {m_CountNumber}");
            first.questItemData.completionCount = m_CountNumber;
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.UI_UPDATE_ITEM_MISSION);
            }
            if (m_CountNumber.Equals(first.questItemData.requestCount))
            {
                currentQuest.isCompleteMission = true;
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.QUEST_COMPLETE, currentQuest.isCompleteMission);
                }
            }
        }
    }

    private void SoundReward()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("RewardItem");
        }
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            questList[i].isCompleteMission = false;
            questList[i].isAcceptMission = false;
            for (int j = 0; j < questList[i].ItemMission.Count; j++)
            {
                questList[i].ItemMission[j].questItemData.completionCount = 0;
            }
        }
        ResetItemCounts();
    }

    private void OnDisable()
    {
        // Khi thoát play mode, reset các số lượng cho các item reward
        ResetItemCounts();
    }
}
