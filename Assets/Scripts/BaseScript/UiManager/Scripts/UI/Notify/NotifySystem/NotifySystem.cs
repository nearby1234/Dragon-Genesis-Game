using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class NotifySystem : BaseNotify
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private TextMeshProUGUI contentNotifyTxt;
    [SerializeField] private NotifySystemData notifySystemData;

    private void Start()
    {
        if(TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.anchoredPosition = offset;
        }

        StartCoroutine(SetHideNotifyQuestOneMission());
        
    }
    public override void Show(object data)
    {
        base.Show(data);
        if (data != null)
        {
            switch (data)
            {
                case NotifySystemData sys:
                    contentNotifyTxt.text = sys.StartGame;
                    // Không gọi SetHideNotify nếu chỉ hiện StartGame?
                    return;

                case NotifyMessageMission<PlayerStamina> stamina:
                    contentNotifyTxt.text = $"<B><color=#FAFF00>{stamina.message}";
                    StartCoroutine(SetHideNotify());
                    break;
                case NotifyMessageMission<PlayerDamage> damage:
                    contentNotifyTxt.text = $"<B>{damage.message}";
                    StartCoroutine(SetHideNotify());
                    break;

                case NotifyMessageMission<NotifyMission> completed:
                    contentNotifyTxt.text = $"Hoàn thành nhiệm vụ <B><color=#0011FF>{completed.questData.questName}";
                    StartCoroutine(SetHideNotify());
                    break;

                case NotifyMessageMission<QuestMissionOnePanel> notify:
                    contentNotifyTxt.text = $"Nhận nhiệm vụ <B><color=#FF0E00>{notify.questData.questName}";
                    StartCoroutine(SetHideNotify());
                    break;

                case NotifyMessageMission<PopupScrollMagic> notify:
                    contentNotifyTxt.text = $"Nhận nhiệm vụ <B><color=#FF0E00>{notify.questData.questName}";
                    StartCoroutine(SetHideNotify());
                    break;
              
                default:
                    
                    return;
            }
           
        }    
    }
    public override void Hide()
    {
       canvasGroup.DOFade(0f, notifySystemData.timeFade).OnComplete(() =>
        {
            base.Hide();
        });
    }
    private void ShowContent(string content)
    {
        canvasGroup.DOFade(1f, notifySystemData.timeFade).OnComplete(() =>
        {
            contentNotifyTxt.text = content;
        });
    }
    IEnumerator SetHideNotifyQuestOneMission()
    {
        yield return new WaitForSeconds(notifySystemData.timeShowContentStartGame);
        Hide();
        ShowContent(notifySystemData.AcceptFirstMission);
        yield return new WaitForSeconds(notifySystemData.timeAcceptFirstMission);
        Hide();
    }    
    IEnumerator SetHideNotify()
    {
        yield return new WaitForSeconds(notifySystemData.timeHideNotify);
        Hide();
    }
    IEnumerator NextNotify(string content)
    {
        yield return new WaitForSeconds(notifySystemData.timeHideNotify);
        contentNotifyTxt.text = content;
    }    
}
