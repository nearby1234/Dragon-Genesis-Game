using System.Collections.Generic;
using UnityEngine;


public class QuestManager : BaseManager<QuestManager>
{
    [SerializeField] private QuestData currentQuest;
    public QuestData CurrentQuest => currentQuest;
    public List<QuestData> questList;
    public  string m_QuestItemPrefabPath = "Prefabs/UI/Quest/ItemMission/ItemMissionImg";
    public  string m_QuestRewardItemPrefabPath = "Prefabs/UI/Quest/ItemReward/ItemRewardImg";
    public  string m_DOItemPrefabPath = "Prefabs/Inventory/DoSpite/DoSpite";

    protected override void Awake()
    {
        base.Awake();
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
    }
    public void AcceptQuest(QuestData quest)
    {
        if(quest != null)
        {
            currentQuest = quest;
            quest.isAcceptMission = true;
            questList.Add(quest);
            
            Debug.Log("Nhận nhiệm vụ: " + currentQuest.questName);
            //UIManager.Instance.ShowScreen<QuestScreen>(currentQuest);
        }
        else
        {
            Debug.LogWarning("Chưa gán QuestData cho nhiệm vụ.");
        }
    }

    public void CompleteQuest()
    {
        if (currentQuest != null)
        {
            Debug.Log("Hoàn thành nhiệm vụ: " + currentQuest.questName);
            // Cấp phần thưởng cho người chơi
            GrantReward(currentQuest.bonus);
            // Hiển thị thông báo hoàn thành nhiệm vụ qua UIManager
            //UIManager.Instance.ShowNotify<QuestNotify>(currentQuest);
        }
    }
   
    private void GrantReward(QuestBonus reward)
    {
        Debug.Log($"Cấp {reward.experience} kinh nghiệm, {reward.gold} vàng.");
        foreach (var item in reward.itemsReward)
        {
            Debug.Log($"Nhận vật phẩm: {item.questItemData.itemName} x{item.questItemData.count}");
            //// Thêm logic để thêm vật phẩm vào kho của người chơi
        }
    }
}
