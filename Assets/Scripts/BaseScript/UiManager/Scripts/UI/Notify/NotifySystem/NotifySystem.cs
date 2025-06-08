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

        StartCoroutine(SetHideNotify());
        
    }
    public override void Show(object data)
    {
        base.Show(data);
        if (data != null)
        {
            if(data is NotifySystemData notifyData)
            {
                contentNotifyTxt.text = notifyData.StartGame;
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


    IEnumerator SetHideNotify()
    {
        yield return new WaitForSeconds(notifySystemData.timeShowContentStartGame);
        Hide();
        ShowContent(notifySystemData.AcceptFirstMission);
        yield return new WaitForSeconds(notifySystemData.timeAcceptFirstMission);
        Hide();
    }    

}
