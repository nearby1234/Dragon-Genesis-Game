using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private QuestData currentQuest;
    public void AcceptQuest(QuestData quest)
    {
        if(currentQuest != null)
        {
            currentQuest = quest;
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
        foreach (var item in reward.items)
        {
            Debug.Log($"Nhận vật phẩm: {item.itemName} x{item.quantity}");
            // Thêm logic để thêm vật phẩm vào kho của người chơi
        }
    }
}
