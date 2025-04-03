using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupTextDamage : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_DamageTxt;
    [SerializeField] private string m_Color;
    private const string m_HashCharacter = "#";
    private const string m_ImagePath = "Image/Sword";


    private void Awake()
    {
        m_DamageTxt = GetComponent<TextMeshPro>();
    }

    public void ChangeTextDamage(int damage, Vector3 position)
    {
        if (m_DamageTxt != null)
        {
            m_DamageTxt.text = $"<color={m_HashCharacter}{m_Color}>{damage}</color>";
        }
        //m_DamageTxt.rectTransform.position = new Vector3(position.x + 2, position.y, position.z);
        m_DamageTxt.rectTransform.position = position;
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        float moveDistance = 2f; // Khoảng cách di chuyển lên
        float duration = 1f; // Thời gian hiệu ứng

        // Di chuyển lên
        m_DamageTxt.rectTransform.DOAnchorPosY(m_DamageTxt.rectTransform.anchoredPosition.y + moveDistance, duration)
            .SetEase(Ease.OutQuad);

        // Mờ dần rồi hủy object
        m_DamageTxt.DOFade(0, duration).OnComplete(() => Destroy(gameObject));
    }
}

