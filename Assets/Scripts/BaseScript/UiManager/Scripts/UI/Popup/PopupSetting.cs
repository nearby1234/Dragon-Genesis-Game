using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : BasePopup
{
    private float bgmVolume;
    private float effectVolume;
    public Slider sliderBGM;
    public Slider sliderEffect;

    public override void Show(object data)
    {
        base.Show(data);
        bgmVolume = PlayerPrefs.GetFloat("BGM",0.75f);
        effectVolume = PlayerPrefs.GetFloat("Effect", 0.75f);
        sliderBGM.value = bgmVolume;
        sliderEffect.value = effectVolume;
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClickCloseButton()
    {
        this.Hide();
    }

    public void OnBGMValueChange(float v)
    {
        bgmVolume = v;
    }

    public void OnEffectValueChange(float v)
    {
        effectVolume = v;
    }

    public void OnApplySetting()
    {
        //if (AudioManager.HasInstance)
        //{
        //    AudioManager.Instance.SetEffectVolume(effectVolume);
        //    AudioManager.Instance.SetBGMVolume(bgmVolume);
        //}
        //this.Hide();
    }
    
}
