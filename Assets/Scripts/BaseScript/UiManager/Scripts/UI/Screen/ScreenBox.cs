using UnityEngine;

public class ScreenBox : BaseScreen
{
    [SerializeField] private Vector2 m_Offset;
    [SerializeField] private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform.anchoredPosition = m_Offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
