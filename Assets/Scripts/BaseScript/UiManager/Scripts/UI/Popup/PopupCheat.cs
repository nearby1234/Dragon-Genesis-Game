using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCheat : BasePopup
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector3 offSet;
    [SerializeField] private TMP_InputField healIptf; // iptf = input field
    [SerializeField] private TMP_InputField ManaIptf;
    [SerializeField] private TMP_InputField StaminaIptf;
    [SerializeField] private TMP_InputField damageIptf;
    [SerializeField] private TMP_Dropdown localDropDown;
    [SerializeField] private Button startBtn;
    [SerializeField] private int limitCharacter = 5;
    [SerializeField] private CreepType creepType;
    [SerializeField] private  BehaviorTreeSO bulltankSO;
    private int healValue, manaValue, damageValue;
    private float staminaValue;

    private void Awake()
    {
        startBtn.onClick.AddListener(OnStartButtonClicked);
        healIptf.onEndEdit.AddListener(OnHealInput);
        ManaIptf.onEndEdit.AddListener(OnManaInput);
        StaminaIptf.onEndEdit.AddListener(OnStaminaInput);
        damageIptf.onEndEdit.AddListener(OnDamageInput);
        localDropDown.onValueChanged.AddListener(OnLocalDropDownChanged);
    }

    private void Start()
    {
        rectTransform.anchoredPosition = offSet;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_CLICK_SHOWUI, null);
        }
    }
    private void OnHealInput(string value)
    {
        healIptf.characterLimit = limitCharacter;
        healValue = int.TryParse(value, out int result) ? result : 0;
    }
    private void OnManaInput(string value)
    {
        ManaIptf.characterLimit = limitCharacter;
        manaValue = int.TryParse(value, out int result) ? result : 0;
    }
    private void OnStaminaInput(string value)
    {
        StaminaIptf.characterLimit = limitCharacter;
        staminaValue = float.TryParse(value, out float result) ? result : 0;
    }
    private void OnDamageInput(string value)
    {
        damageIptf.characterLimit = limitCharacter;
        damageValue = int.TryParse(value, out int result) ? result : 0;
    }
    private void OnLocalDropDownChanged(int value)
    {
        Debug.Log($"Selected value: {value}");
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        switch (value)
        {
            case 1:
                creepType = CreepType.WORM;
                break;
            case 2:
                creepType = CreepType.BullTank;
                break;
        }
    }
    private void OnStartButtonClicked()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
            ListenerManager.Instance.BroadCast(ListenType.CHEAT_PLAYER_HEAL, healValue);
            ListenerManager.Instance.BroadCast(ListenType.CHEAT_PLAYER_MANA, manaValue);
            ListenerManager.Instance.BroadCast(ListenType.CHEAT_PLAYER_STAMINA, staminaValue);
            ListenerManager.Instance.BroadCast(ListenType.CHEAT_PLAYER_DAMAGE, damageValue);
            ListenerManager.Instance.BroadCast(ListenType.CREEP_TYPE, creepType);
            if(creepType == CreepType.BullTank)
            {
                SendBullTankHeal();
            }
            if(GameManager.HasInstance)
            {
                GameManager.Instance.HideCursor();
            }
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("ClickSound");
            }
            this.Hide();
        }
    }
    private void SendBullTankHeal()
    {
        DataBullTankBoss dataBullTank = new()
        {
            creepType = this.creepType,
            m_Heal = bulltankSO.Heal,
        };
        ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_SEND_HEAL_VALUE, dataBullTank);
    }
}
