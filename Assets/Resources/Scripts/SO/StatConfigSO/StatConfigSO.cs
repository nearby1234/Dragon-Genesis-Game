using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New StatConfigSO", menuName = "Scriptable Object/StatConfigSO")]
public class StatConfigSO : ScriptableObject
{
    [Tooltip("Name của GameObject/Component (ví dụ: 'StrengthStatsTxt')")]
    public string statKey;

    [Tooltip("Tên hiển thị trên UI (ví dụ: 'Sức Mạnh')")]
    public string displayName;

    [Tooltip("Key của TextMeshProUGUI dùng để preview (ví dụ: 'DamageTxt' cho StrengthStatsTxt)")]
    public string previewUITextKey;

    [Tooltip("Điểm mặc định ban đầu")]
    public int defaultPoint = 0;

    [Tooltip("Điểm tối đa có thể cộng")]
    public int maxPoint = 100;

    [Tooltip("Các multiplier sẽ được gán runtime từ PlayerManager: ví dụ PlusHealValue, PlusManaValue...")]
    public int multiplier;
}
