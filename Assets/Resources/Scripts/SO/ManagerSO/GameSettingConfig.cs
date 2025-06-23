using System.Collections.Generic;
using UnityEngine;


public enum GameSetting
{
    GameSetting
}
[CreateAssetMenu(fileName = "GameSettingConfig", menuName = "Scriptable Objects/GameSettingConfig")]
[System.Serializable]
public class GameSettingConfig : ScriptableObject , IEnumKeyed<GameSetting> 
{
    public GameSetting Key => gameSetting;
    public GameSetting gameSetting;

    [Header("Setting Amount Reward Mission")]
    public List<SetAmountItemReward> setAmountItemRewards = new();

   
}
[System.Serializable]
public class SetAmountItemReward
{
    public int CountItemRewards;
    public int AmountAsAccept;
    public int AmountAsReject;
}
