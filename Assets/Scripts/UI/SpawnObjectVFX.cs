using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnObjectVFX : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private GameObject m_ParentTranformObj;
    [SerializeField] private GameObject m_SpawnVFXPrefab;
    [SerializeField] private Slider m_UiTarget;
    [SerializeField] private Animator animator;
    [SerializeField] private Canvas mainCanvas; // Canvas chính đang chứa UI
    [SerializeField] private Canvas m_PatCanvas; // Slider exp trong canvas chính
    [SerializeField] private Camera uiCamera;  // Camera dùng cho canvas (nếu sử dụng Render Mode Screen Space - Camera)
    [SerializeField] private int m_PoolSize = 10;
    [SerializeField] private Queue<GameObject> poolVfx = new();
    [SerializeField] private RectTransform sliderRectTransform;
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
            ListenerManager.Instance.Register(ListenType.CAMERA_SEND_VALUE, ReceiverCamera);
            ListenerManager.Instance.BroadCast(ListenType.UI_SEND_CANVASMAIN, mainCanvas);
        }
        InitPoolVFX();
        StartCoroutine(DelayGetRectTransform());
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_SCREEN_SLIDER_EXP, ReceiverValueTransformSLider);
            ListenerManager.Instance.Unregister(ListenType.CAMERA_SEND_VALUE, ReceiverCamera);
        }
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public GameObject GetObjectFormPool(int countExpOrb)  // Gọi hàm này khi muốn tạo exp move lên slider exp
    {
        if (poolVfx.Count > 0)
        {
            for (int i = 0; i < countExpOrb; i++)
            {
                if (poolVfx.Count > 0)
                {
                    GameObject vfx = poolVfx.Dequeue();
                    RectTransform rectTransform = vfx.GetComponent<RectTransform>();
                    vfx.SetActive(true);
                    MoveSLiderUI(rectTransform);
                    return vfx;
                }
            }
        }
        else
        {
            Debug.Log("No VFX available in the pool.");
            GameObject vfx = Instantiate(m_SpawnVFXPrefab, m_ParentTranformObj.transform);
            return vfx;
        }
        return null;
    }
    public void ReturnObjectToPool(RectTransform rectTransform)
    {
        rectTransform.gameObject.SetActive(false);
        rectTransform.anchoredPosition = Vector2.zero;
        poolVfx.Enqueue(rectTransform.gameObject);
    }
    public Vector2 TranslatePosition()
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

    public void PlayAnimationFade()
    {
        if (animator != null)
        {
            animator.Play("Fade");
        }
    }    

    private void InitPoolVFX()
    {
        Debug.Log("InitPoolVFX");
        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject vfx = Instantiate(m_SpawnVFXPrefab, m_ParentTranformObj.transform);
            
            vfx.SetActive(false);
            poolVfx.Enqueue(vfx);
        }
    }
    private void ReceiverValueTransformSLider(object value)
    {
        if (value is Slider slider)
        {
            m_UiTarget = slider;
            sliderRectTransform = slider.GetComponent<RectTransform>();
            animator = slider.GetComponentInParent<Animator>();

            // Lấy canvas parent
            m_PatCanvas = this.GetComponent<Canvas>();
            // Chuyển sang ScreenSpaceCamera nếu cần
            m_PatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            m_PatCanvas.worldCamera = uiCamera;   // uiCamera đã được set trong ReceiverCamera
        }
    }
    private void MoveSLiderUI(RectTransform rectTransform)
    {
        Vector2 sliderRectPos = TranslatePosition();
        Sequence seq = DOTween.Sequence();

        seq.Append(rectTransform.DOAnchorPos(sliderRectPos, m_Speed)
            .SetEase(Ease.InBack));
        seq.Join(rectTransform.DOScale(Vector3.zero, m_Speed).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            if (PlayerLevelManager.HasInstance)
            {
                PlayerLevelManager.Instance.AddExp(PlayerLevelManager.Instance.ExpOrbValue);
            }
            PlayAnimationFade();
            ReturnObjectToPool(rectTransform);
        } 
        );

        //rectTransform.DOAnchorPos(sliderRectPos, m_Speed)
        //    .SetEase(Ease.InBack)
        //    .OnComplete(() =>
        //    {
                
        //    });
    }

   
    private IEnumerator DelayGetRectTransform()
    {
        // 1. Chờ game state chuyển qua khỏi MENULOADING
        while (GameManager.HasInstance && GameManager.Instance.GameState == GAMESTATE.MENULOADING)
        {
            yield return new WaitForSeconds(1f);
        }

        // 2. Chờ đến khi m_UiTarget được gán (tức UI đã khởi tạo xong)
        while (m_UiTarget == null)
        {
            yield return new WaitForSeconds(0.5f); // kiểm tra mỗi nửa giây
        }

        // 3. Lúc này đã có thể lấy RectTransform an toàn
        //sliderRectTransform = m_UiTarget.GetComponent<RectTransform>();

        // 4. Nếu có Animator trong parent, lấy luôn (kèm null-check)
        if (m_UiTarget.GetComponentInParent<Animator>() != null)
        {
            animator = m_UiTarget.GetComponentInParent<Animator>();
        }
    }
    private void ReceiverCamera(object value)
    {
        if (value is Camera cam)
            uiCamera = cam;
    }
   
}
