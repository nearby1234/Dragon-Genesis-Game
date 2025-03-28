using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class NotifyLoading : BaseNotify
{
    public TextMeshProUGUI tmpLoading;
    public Slider slProgress;
    private string loadingText = "Loading";

    public override void Hide()
    {
        base.Hide();
    }

    public override void Show(object data)
    {
        base.Show(data);
    }

    public void SetProgress(float dt)
    {
        this.slProgress.value = dt;
    }

    public void AnimationLoaddingText()
    {
        DOTween.Kill(this.tmpLoading.GetInstanceID().ToString());
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.tmpLoading.GetInstanceID().ToString());
        seq.AppendCallback(() =>
        {
            this.tmpLoading.text = loadingText;
        });
        seq.AppendInterval(5 / 4f);
        seq.AppendCallback(() =>
        {
            this.tmpLoading.text = loadingText + ".";
        });
        seq.AppendInterval(5 / 4f);
        seq.AppendCallback(() =>
        {
            this.tmpLoading.text = loadingText + "..";
        });
        seq.AppendInterval(5 / 4f);
        seq.AppendCallback(() =>
        {
            this.tmpLoading.text = loadingText + "...";
        });
        seq.AppendInterval(5 / 4f);
        seq.SetId(-1);
    }

    public void DoAnimationLoadingProgress(float dt, Action OnComplete = null)
    {
        DOTween.Kill(this.slProgress.GetInstanceID().ToString());
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.slProgress.GetInstanceID().ToString());
        SetProgress(0);
        seq.Append(this.slProgress.DOValue(slProgress.maxValue, dt).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
}
