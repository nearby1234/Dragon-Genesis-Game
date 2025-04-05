using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventory : BasePopup
{
    [SerializeField] private int m_CountBox;
    [SerializeField] private GameObject m_BoxInventory;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private TextMeshProUGUI m_MoneyTxt;
    [SerializeField] private Transform m_InventoryPanel;
    [SerializeField] private List<Sprite> m_ImageIconList = new();
    [SerializeField] private List<InventorySlot> listBoxInventory = new();
    
  
    private const string m_BoxImgPath = "Prefabs/Inventory/BoxImg";

    private void Awake()
    {
        m_BoxInventory = Resources.Load<GameObject>(m_BoxImgPath);
        InitBoxInventory();
    }
    private void Start()
    {
        AddItems(m_ImageIconList);
    }


    private void InitBoxInventory()
    {
        for (int i = 0; i < m_CountBox; i++)
        {
            GameObject box = Instantiate(m_BoxInventory, m_InventoryPanel);
            box.name = $"{box.name}-{i}";
            InventorySlot canvasGroup = box.GetComponentInChildren<InventorySlot>();
            canvasGroup.name = $"{canvasGroup.name}-{i}";
            listBoxInventory.Add(canvasGroup);
        }
    }

    private void AddItems(List<Sprite> sprites)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            if (i < listBoxInventory.Count) // ??m b?o không v??t quá s? l??ng ô trong listBoxInventory
            {
                listBoxInventory[i].SetItemSprite(sprites[i]);
            }
        }
    }

}
