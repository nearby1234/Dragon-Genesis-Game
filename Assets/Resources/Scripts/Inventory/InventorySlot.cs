using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(DragDropItem))]
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private bool m_IsEmpty = true;
    [SerializeField] private Image m_IconImage;
    [SerializeField] private TextMeshProUGUI m_CountTxt;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InputAction m_ButtonPress;
    [InlineEditor]
    public QuestItemSO m_CurrentItem;
    private Queue<GameObject> m_ItemPool = new Queue<GameObject>();
    public bool IsEmpty
    {
        get => m_IsEmpty;
        set => m_IsEmpty = value;
    }
    private void Awake()
    {
        if (m_IconImage == null)
        {
            m_IconImage = GetComponent<Image>();
        }
        if (m_CountTxt == null)
        {
            m_CountTxt = GetComponentInChildren<TextMeshProUGUI>();
        }
        m_Slider = GetComponentInChildren<Slider>();
        canvasGroup = m_Slider.GetComponent<CanvasGroup>();


    }
    private void Start()
    {
        m_ButtonPress.Enable();
        m_ButtonPress.performed += OnPerformPressButton;
        //SetStateSliderCoolDownTime(false);
    }
    private void OnDestroy()
    {
        m_ButtonPress.performed -= OnPerformPressButton;
        m_ButtonPress.Disable();
       
    }
    public void SetItemSprite(QuestItemSO sprite)
    {
        if (sprite == null) return;
        m_CurrentItem = sprite; // Cập nhật biến lưu trữ item hiện tại
        m_IconImage.sprite = sprite.questItemData.icon;
        SetAlphaColor(1f);
        UpdateCountText(sprite.questItemData.count);
    }
    public void ClearItem()
    {
        m_IconImage.sprite = null;
        SetAlphaColor(0f);
        m_IsEmpty = true;
    }
    private void SetAlphaColor(float alpha)
    {
        if (m_IconImage == null) return;

        Color temp = m_IconImage.color;
        temp.a = alpha;
        m_IconImage.color = temp;
    }
    private void UpdateCountText(int count)
    {
        m_CountTxt.color = new Color(1, 1, 1, 1); // Đặt màu chữ thành trắng
        //m_CountTxt.enabled = true;
        m_CountTxt.text = count.ToString();
    }
    private void OnPerformPressButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (m_CurrentItem.questItemData.typeItem)
            {
                case TYPEITEM.ITEM_USE:
                    {
                        // kiểm tra xem item có thể sử dụng hay không
                        CheckCountItem();
                        UseItem();
                    }
                    // Thực hiện hành động cho ITEM_MISSION
                    Debug.Log("Thực hiện hành động cho ITEM_MISSION");
                    break;
                case TYPEITEM.ITEM_EQUIP:
                    // Thực hiện hành động cho ITEM_EQUIP
                    Debug.Log("Thực hiện hành động cho ITEM_EQUIP");
                    break;
                case TYPEITEM.ITEM_COLLECT:
                    // Thực hiện hành động cho ITEM_COLLECT
                    Debug.Log("Thực hiện hành động cho ITEM_COLLECT");
                    break;
                default:
                    Debug.Log("Không có hành động nào được xác định cho loại item này.");
                    break;
            }
        }
    }
    private void CheckCountItem()
    {

        // Kiểm tra xem item có thể sử dụng hay không
        if (m_CurrentItem.questItemData.count > 0)
        {
            // Giảm số lượng item
            m_CurrentItem.questItemData.count--;
            UpdateCountText(m_CurrentItem.questItemData.count);
            // Thực hiện hành động sử dụng item
        }
        else
        {
            Debug.Log("Không đủ số lượng item để sử dụng.");
        }
    }
    private void UseItem()
    {
        switch (m_CurrentItem.questItemData.itemUse)
        {
            case ITEMUSE.ITEM_USE_HEAL:
                if (ListenerManager.HasInstance)
                {
                    Debug.Log("Thực hiện hành động hồi phục");
                    ListenerManager.Instance.BroadCast(ListenType.ITEM_USE_DATA_IS_HEAL, m_CurrentItem.questItemData.percentIncrease);
                }
                if (EffectManager.HasInstance)
                {
                    GameObject heal = GetPooledItem("Heal", PlayerManager.instance.transform); // hoặc transform cha
                    CooldownTime(m_CurrentItem.questItemData.timeCoolDown);
                }
                break;
        }
    }
    private void CooldownTime(float time)
    {
        SetStateSliderCoolDownTime(true);
        m_Slider.value = 0;
        m_Slider.DOValue(1, time).OnComplete(() =>
        {
            SetStateSliderCoolDownTime(false);
        });
    }
    private void SetStateSliderCoolDownTime(bool isActive)
    {

        if (isActive)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


    }
    private GameObject GetPooledItem(string effectName, Transform parent)
    {
        GameObject item = null;

        if (m_ItemPool.Count > 0)
        {
            // Lấy object từ pool nếu có
            item = m_ItemPool.Dequeue();
            item.SetActive(true);
        }
        else
        {
            // Nếu pool rỗng thì mới instantiate
            if (EffectManager.HasInstance)
            {
                GameObject prefab = EffectManager.Instance.GetPrefabs(effectName);
                if (prefab != null)
                {
                    item = Instantiate(prefab, parent);
                }
            }
        }

        if (item != null)
        {
            // Reset lại trạng thái transform cho dù item lấy từ pool hay vừa được instantiate
            item.transform.SetParent(parent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            // Đối với ParticleSystem, đảm bảo thực hiện reset và play
            ParticleSystem ps = item.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                ps.Clear();
                ps.Play();
            }
        }
        return item;
    }

    private IEnumerator ReturnToPoolAfterPlay(ParticleSystem ps, GameObject item)
    {
        yield return new WaitUntil(() => !ps.IsAlive(true));
        ReturnPooledItem(item);
    }
    private void ReturnPooledItem(GameObject item)
    {
       if(item != null)
        {
            item.SetActive(false);
            item.transform.SetParent(null); // Đặt lại parent về null hoặc về một đối tượng khác nếu cần
            m_ItemPool.Enqueue(item);
        }
    }
}
