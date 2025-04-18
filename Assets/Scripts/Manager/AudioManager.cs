using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager<AudioManager>
{
    private float bgmFadeSpeedRate;/* = CONST.BGM_FADE_SPEED_RATE_HIGH;*/

    //Next BGM name, SE name
    private string nextBGMName;
    private string nextSEName;

    //Is the background music fading out?
    private bool isFadeOut = false;
    private float fadeInSpeedRate;
    private bool isFadeIn = false;
    private float targetVolume = 1f; // volume cuối cùng bạn muốn đạt tới


    //Separate audio sources for BGM and SE
    public AudioSource AttachBGMSource;
    public AudioSource AttachSESource;

    //Keep All Audio
    [ShowInInspector]
    private Dictionary<string, AudioClip> bgmDic = new();
    [ShowInInspector]
    private Dictionary<string, AudioClip> seDic = new();

    protected override void Awake()
    {
        base.Awake();
        //Load all SE & BGM files from resource folder
        //bgmDic = new Dictionary<string, AudioClip>();
        //seDic = new Dictionary<string, AudioClip>();

        object[] bgmList = Resources.LoadAll("Audio/BGM");
        object[] seList = Resources.LoadAll("Audio/SE");

        foreach (AudioClip bgm in bgmList)
        {
            bgmDic[bgm.name] = bgm;
        }
        foreach (AudioClip se in seList)
        {
            seDic[se.name] = se;
        }
    }

    private void Start()
    {
        //AttachBGMSource.volume = ObscuredPrefs.GetFloat(CONST.BGM_VOLUME_KEY, CONST.BGM_VOLUME_DEFAULT);
        //AttachSESource.volume = ObscuredPrefs.GetFloat(CONST.SE_VOLUME_KEY, CONST.SE_VOLUME_DEFAULT);
        //AttachBGMSource.mute = ObscuredPrefs.GetBool(CONST.BGM_MUTE_KEY, CONST.BGM_MUTE_DEFAULT);
        //AttachSESource.mute = ObscuredPrefs.GetBool(CONST.SE_MUTE_KEY, CONST.SE_MUTE_DEFAULT);
    }

    public void PlaySE(string seName, float delay = 0.0f)
    {
        if (!seDic.ContainsKey(seName))
        {
            Debug.Log(seName + "There is no SE named");
            return;
        }

        nextSEName = seName;
        Invoke("DelayPlaySE", delay);
    }

    private void DelayPlaySE()
    {
        AttachSESource.PlayOneShot(seDic[nextSEName] as AudioClip);
    }

    public void PlayBGM(string bgmName/*, float fadeSpeedRate = CONST.BGM_FADE_SPEED_RATE_HIGH*/)
    {
        if (!bgmDic.ContainsKey(bgmName))
        {
            Debug.Log(bgmName + "There is no BGM named");
            return;
        }

        //If BGM is not currently playing, play it as is
        if (!AttachBGMSource.isPlaying)
        {
            nextBGMName = "";
            AttachBGMSource.clip = bgmDic[bgmName] as AudioClip;
            AttachBGMSource.Play();
        }
        //When a different BGM is playing, fade out the BGM that is playing before playing the next one.
        //Ignore when the same BGM is playing
        else if (AttachBGMSource.clip.name != bgmName)
        {
            nextBGMName = bgmName;
            FadeOutBGM(1f, 0.5f);
        }

    }

    public void FadeOutBGM(float fadeOutSpeed, float fadeInSpeed = 1f)
    {
        bgmFadeSpeedRate = fadeOutSpeed;
        fadeInSpeedRate = fadeInSpeed;
        // lưu lại volume hiện tại để target
        targetVolume = AttachBGMSource.volume;
        isFadeOut = true;
    }

    private void Update()
    {
        // xử lý fade‑out
        if (isFadeOut)
        {
            AttachBGMSource.volume -= Time.deltaTime * bgmFadeSpeedRate;
            if (AttachBGMSource.volume <= 0f)
            {
                AttachBGMSource.Stop();
                isFadeOut = false;

                if (!string.IsNullOrEmpty(nextBGMName))
                {
                    // gán clip mới và play
                    AttachBGMSource.clip = bgmDic[nextBGMName];
                    AttachBGMSource.Play();
                    // bắt đầu fade‑in
                    AttachBGMSource.volume = 0f;
                    isFadeIn = true;
                }
            }
            return;  // ưu tiên fade‑out trước
        }

        // xử lý fade‑in
        if (isFadeIn)
        {
            AttachBGMSource.volume += Time.deltaTime * fadeInSpeedRate;
            if (AttachBGMSource.volume >= targetVolume)
            {
                AttachBGMSource.volume = targetVolume;
                isFadeIn = false;
            }
        }
    }

    //public void ChangeBGMVolume(float BGMVolume)
    //{
    //    AttachBGMSource.volume = BGMVolume;
    //    ObscuredPrefs.SetFloat(CONST.BGM_VOLUME_KEY, BGMVolume);
    //}

    //public void ChangeSEVolume(float SEVolume)
    //{
    //    AttachSESource.volume = SEVolume;
    //    ObscuredPrefs.SetFloat(CONST.SE_VOLUME_KEY, SEVolume);
    //}

    //public void MuteBGM(bool isMute)
    //{
    //    AttachBGMSource.mute = isMute;
    //    ObscuredPrefs.SetBool(CONST.BGM_MUTE_KEY, isMute);
    //}

    //public void MuteSE(bool isMute)
    //{
    //    AttachSESource.mute = isMute;
    //    ObscuredPrefs.SetBool(CONST.SE_MUTE_KEY, isMute);
    //}
}
