using UnityEngine;
using UnityEngine.UI;

public class PopupSettingBoxImg : BasePopup
{
    [SerializeField] private Slider m_SoundSlider;
    [SerializeField] private Toggle m_SoundToggle;
    [SerializeField] private Slider m_MusicSlider;
    [SerializeField] private Toggle m_MusicToggle;
    [SerializeField] private Button m_ExitButton;
    [SerializeField] private AudioSource m_SoundAudio;
    [SerializeField] private AudioSource m_MusicAudio;

    private const string SOUND_KEY = "SoundVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const float DEFAULT_VOLUME = 0.5f;

    private void Start()
    {
        if (AudioManager.HasInstance)
        {
            m_SoundAudio = AudioManager.Instance.AttachSESource;
            m_MusicAudio = AudioManager.Instance.AttachBGMSource;
        }

        m_SoundToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, m_SoundToggle, m_SoundAudio, m_SoundSlider, SOUND_KEY));
        m_MusicToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, m_MusicToggle, m_MusicAudio, m_MusicSlider, MUSIC_KEY));
        m_ExitButton.onClick.AddListener(OnClickButton);
    }

    private void Update()
    {
        UpdateValueSlider(m_MusicSlider, m_MusicAudio, m_MusicToggle);
        UpdateValueSlider(m_SoundSlider, m_SoundAudio, m_SoundToggle);
    }

    private void OnToggleChanged(bool isOn, Toggle toggle, AudioSource audioSource, Slider slider, string key)
    {
        SetAlphaCheckMask(toggle, isOn);
        SetValueSlider(audioSource, slider, isOn, key);
    }
    private void OnClickButton()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        Hide();
    }    

    private void SetAlphaCheckMask(Toggle toggle, bool isOn)
    {
        toggle.isOn = isOn;
        Color activeColor = new(1, 1, 1, 1f);
        Color inactiveColor = new(1, 1, 1, 0f);

        toggle.graphic.color = isOn ? activeColor : inactiveColor;
        toggle.targetGraphic.color = isOn ? inactiveColor : activeColor;
    }

    private void SetValueSlider(AudioSource audioSource, Slider slider, bool isOn, string key)
    {
        if (isOn)
        {
            PlayerPrefs.SetFloat(key, slider.value);
            slider.value = 0;
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat(key, DEFAULT_VOLUME);
        }
        SetAudio(audioSource, slider.value);
    }

    private void SetAudio(AudioSource audioSource, float value)
    {
        audioSource.volume = value;
    }

    private void UpdateValueSlider(Slider slider, AudioSource audioSource, Toggle toggle)
    {
        audioSource.volume = slider.value;
        toggle.isOn = slider.value <= 0f;
    }
    
}
