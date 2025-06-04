using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SetupTextDamage : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_DamageTxt;
    [SerializeField] private string m_Color;
    private Camera Camera;


    private void Awake()
    {
        m_DamageTxt = GetComponent<TextMeshPro>();
    }
    private void Start()
    {
        Camera = Camera.main;   
    }

    private void LateUpdate()
    {
        RotateforwarCamera();
    }
    public void ChangeTextDamage(int damage, Vector3 position)
    {
        if (m_DamageTxt != null)
        {
            m_DamageTxt.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            m_DamageTxt.text = damage.ToString();
        }
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

    private void RotateforwarCamera()
    {
        if(Camera !=null)
        {
            transform.LookAt(Camera.transform);
            transform.rotation = Quaternion.LookRotation(Camera.transform.forward);
        }    
    }
}

