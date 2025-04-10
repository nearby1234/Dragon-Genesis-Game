using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnObjectVFX : MonoBehaviour
{
    [SerializeField] private int m_ValueExp;
    [SerializeField] private float m_Speed;
    [SerializeField] private GameObject m_ParentTranformObj;
    [SerializeField] private GameObject m_SpawnVFXPrefab;
    [SerializeField] private Slider m_UiTarget;
    [SerializeField] private RectTransform sliderRectTransform;
    [SerializeField] private Canvas mainCanvas; // Canvas chính đang chứa UI
    [SerializeField] private Camera uiCamera;  // Camera dùng cho canvas (nếu sử dụng Render Mode Screen Space - Camera)
    [SerializeField] private int m_PoolSize = 10;
    [SerializeField] private Queue<GameObject> poolVfx = new();
    [ShowInInspector, ReadOnly]
    private List<GameObject> vfxPoolList => new(poolVfx);

    private void Start()
    {
        if (EffectManager.HasInstance)
        {
            m_SpawnVFXPrefab =EffectManager.Instance.GetPrefabs("GlowingOrbExp");
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.UI_SEND_SCREEN_SLIDER_EXP, ReceiverValueTransformSLider);
           
        }
        StartCoroutine(DelayGetRectTransform());
        InitPoolVFX();
       
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_SCREEN_SLIDER_EXP, ReceiverValueTransformSLider);
        }
    }
    public GameObject GetObjectFormPool()
    {
        if (poolVfx.Count > 0)
        {
            GameObject vfx = poolVfx.Dequeue();
            RectTransform rectTransform = vfx.GetComponent<RectTransform>();
            vfx.SetActive(true);
            MoveSLiderUI(rectTransform);
            return vfx;
        }
        else
        {
            GameObject vfx = Instantiate(m_SpawnVFXPrefab, m_ParentTranformObj.transform);
            return vfx;
        }
    }
    public void ReturnObjectToPool(RectTransform rectTransform)
    {
        rectTransform.gameObject.SetActive(false);
        rectTransform.anchoredPosition = Vector2.zero;
        poolVfx.Enqueue(rectTransform.gameObject);
    }

    private void InitPoolVFX()
    {
        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject vfx = Instantiate(m_SpawnVFXPrefab, m_ParentTranformObj.transform);
            
            vfx.SetActive(false);
            poolVfx.Enqueue(vfx);
        }
    }
    private void ReceiverValueTransformSLider(object value)
    {
        if (value != null && value is Slider sliderTransform)
        {
            m_UiTarget = sliderTransform;
        }
    }
    private void MoveSLiderUI(RectTransform rectTransform)
    {
       Vector2 sliderRectPos = TranslatePosition();
        rectTransform.DOAnchorPos(sliderRectPos, m_Speed)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if(ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.UI_SEND_VALUE_EXP_TO_SLIDER, m_ValueExp);
                }
                ReturnObjectToPool(rectTransform);
            });
    }

    private Vector2 TranslatePosition()
    {
        // Lấy tọa độ world của slider
        Vector3 worldPos = sliderRectTransform.position;
        // Chuyển world position sang screen position
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);
        // Chuyển screen position thành local position theo canvas chính
        Vector2 canvasLocalPos;
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        // Nếu canvas có camera (Screen Space - Camera) thì truyền vào, nếu không (Overlay) truyền null
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out canvasLocalPos);
        return canvasLocalPos;
    }
    private IEnumerator DelayGetRectTransform()
    {
        yield return new WaitForEndOfFrame();
        sliderRectTransform = m_UiTarget.GetComponent<RectTransform>();
    }
}
