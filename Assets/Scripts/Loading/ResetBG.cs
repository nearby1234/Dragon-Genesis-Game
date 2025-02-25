using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ResetBG : MonoBehaviour
{
    [SerializeField] private Image m_BG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_BG.DOFade(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
