using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NotifyMission : BaseNotify
{
    [SerializeField] private QuestData questDataCurrent;
    [SerializeField] private Image missionImg;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionTitleText;
    [SerializeField] private Vector3 offSet;

    private void Start()
    {
        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.anchoredPosition = offSet;
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_QUESTMISSION_CURRENT, OnReceiveQuestMission);
            ListenerManager.Instance.Register(ListenType.UI_UPDATE_ITEM_MISSION, OnReceiveUiUpdateItem);
            ListenerManager.Instance.Register(ListenType.QUEST_COMPLETE,OnReceiveMissionComplete);
        }
        HandlerShowMission(false);
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_QUESTMISSION_CURRENT, OnReceiveQuestMission);
            ListenerManager.Instance.Unregister(ListenType.UI_UPDATE_ITEM_MISSION, OnReceiveUiUpdateItem);
            ListenerManager.Instance.Unregister(ListenType.QUEST_COMPLETE, OnReceiveMissionComplete);
        }
    }
    private void OnReceiveQuestMission(object value)
    {
        if (value is QuestData questData)
        {
            questDataCurrent = questData;
            HandlerShowMission(true);
            UpdateMission(false);
        }
    }
    private void OnReceiveUiUpdateItem(object value)
    {
        if (QuestManager.HasInstance)
        {
            questDataCurrent = QuestManager.Instance.CurrentQuest;
            UpdateMission(false);
        }
    }
    private void OnReceiveMissionComplete(object value)
    {
        if (value !=null)
        {
            if(value is bool isComplete)
            {
                if(isComplete)
                {
                    questDataCurrent.isCompleteMission = true;
                    UpdateMission(isComplete);
                }
               
            }    
        }
    }    

    private void UpdateMission(bool isComplete)
    {
        foreach (var item in questDataCurrent.ItemMission)
        {
            missionImg.sprite = item.questItemData.icon;
            if (isComplete)
            {
                missionText.text = $"<B><color=#03FF00> {item.questItemData.itemName} {item.questItemData.completionCount}/{item.questItemData.requestCount}";
            }
            else
            {
                missionText.text = $"{item.questItemData.itemName} {item.questItemData.completionCount}/{item.questItemData.requestCount}";
            }

        }
    }
    private void HandlerShowMission(bool isShow)
    {
        missionImg.gameObject.SetActive(isShow);
        missionText.gameObject.SetActive(isShow);
        missionTitleText.gameObject.SetActive(isShow);
    }

}
