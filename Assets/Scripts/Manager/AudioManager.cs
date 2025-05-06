using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : BaseManager<AudioManager>
{
    // Fade control
    private float bgmFadeOutRate;
    private float bgmFadeInRate;
    private float targetVolume = 1f;
    private bool isFadeOut = false;
    private bool isFadeIn = false;
    private string nextBGMName;

    // Audio sources
    public AudioSource AttachBGMSource;
    public AudioSource AttachSESource;
    public AudioSource loopSESource;
    public AudioSource FootstepSource;

    // Loaded clips
    [ShowInInspector]
    private Dictionary<string, AudioClip> bgmDic = new();
    [ShowInInspector]
    private Dictionary<string, AudioClip> seDic = new();

    protected override void Awake()
    {
        base.Awake();
        // Load all clips from Resources
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/BGM"))
            bgmDic[clip.name] = clip;
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/SE"))
            seDic[clip.name] = clip;
    }

    public void PlaySE(string seName, float delay = 0f)
    {
        if (!seDic.ContainsKey(seName))
        {
            Debug.LogWarning($"SE not found: {seName}");
            return;
        }
        nextBGMName = "";
        nextSEName = seName;
        Invoke(nameof(DelayPlaySE), delay);
    }

    private string nextSEName;
    private void DelayPlaySE()
    {
        AudioClip clip = seDic[nextSEName];
        if (AttachSESource.loop)
        {
            // loop: gán clip chính, gọi Play()
            AttachSESource.clip = clip;
            AttachSESource.Play();
        }
        else
        {
            // one-shot: PlayOneShot bình thường
            AttachSESource.PlayOneShot(clip);
        }
    }
    /// <summary>
    /// Plays or fades to BGM. If force==true, immediately stops and plays.
    /// </summary>
    public void PlayBGM(string bgmName, bool force = false)
    {
        if (!bgmDic.ContainsKey(bgmName))
        {
            Debug.LogWarning($"BGM not found: {bgmName}");
            return;
        }
        bool sameClip = AttachBGMSource.clip != null && AttachBGMSource.clip.name == bgmName;

        if (force)
        {
            // Force restart
            AttachBGMSource.Stop();
            isFadeOut = isFadeIn = false;
            AttachBGMSource.clip = bgmDic[bgmName];
            AttachBGMSource.Play();
            return;
        }

        if (!AttachBGMSource.isPlaying)
        {
            // No music playing -> play immediately
            AttachBGMSource.clip = bgmDic[bgmName];
            AttachBGMSource.Play();
            nextBGMName = "";
        }
        else if (!sameClip)
        {
            // Different music playing -> fade out then play
            nextBGMName = bgmName;
            FadeOutBGM(1f, 0.5f);
        }
        // else: same clip already playing -> do nothing
    }

    public void FadeOutBGM(float fadeOutSpeed, float fadeInSpeed = 1f)
    {
        bgmFadeOutRate = fadeOutSpeed;
        bgmFadeInRate = fadeInSpeed;
        targetVolume = AttachBGMSource.volume;
        isFadeOut = true;
    }
    public void PlayLoopSE(string name, float volume = 1f)
    {
        if (seDic.TryGetValue(name, out AudioClip clip))
        {
            if (loopSESource.isPlaying && loopSESource.clip == clip)
                return;

            loopSESource.clip = clip;
            loopSESource.volume = volume;
            loopSESource.loop = true;
            loopSESource.Play();
        }
    }

    public void StopLoopSE()
    {
        if (loopSESource.isPlaying)
        {
            loopSESource.Stop();
        }
    }

    private void Update()
    {
        if (isFadeOut)
        {
            AttachBGMSource.volume -= Time.deltaTime * bgmFadeOutRate;
            if (AttachBGMSource.volume <= 0f)
            {
                AttachBGMSource.Stop();
                isFadeOut = false;
                if (!string.IsNullOrEmpty(nextBGMName))
                {
                    AttachBGMSource.clip = bgmDic[nextBGMName];
                    AttachBGMSource.Play();
                    AttachBGMSource.volume = 0f;
                    isFadeIn = true;
                }
            }
            return;
        }
        if (isFadeIn)
        {
            AttachBGMSource.volume += Time.deltaTime * bgmFadeInRate;
            if (AttachBGMSource.volume >= targetVolume)
            {
                AttachBGMSource.volume = targetVolume;
                isFadeIn = false;
            }
        }
    }


    public void PlayPlayerSound(string footstepSE, float volume = 1f)
    {
        if (!seDic.ContainsKey(footstepSE)) return;

        FootstepSource.volume = Mathf.Clamp01(volume); // gán trực tiếp volume
        FootstepSource.PlayOneShot(seDic[footstepSE]);
    }


    public void SetPlayerVolume(float volume)
    {
        var mixerGroup = FootstepSource.outputAudioMixerGroup;
        if (mixerGroup != null ? mixerGroup.audioMixer : null != null)
        {
            float linear = Mathf.Clamp(volume, 0.0001f, 1f);
            float dB = Mathf.Log10(linear) * 20f;
            mixerGroup.audioMixer.SetFloat("FootstepVolume", dB);
        }
    }

    public void PlayEnemySound(string enemysound, Vector3 position, float volume = 1f)
    {
        if (!seDic.ContainsKey(enemysound))
        {
            Debug.LogWarning($"không có {enemysound} trong dict");
            return;
        }
        
        AudioSource.PlayClipAtPoint(seDic[enemysound], position, volume);
    }
}
