using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;
using Sirenix.Utilities;
using Sirenix.OdinInspector;

public class PlayerLevelManager : BaseManager<PlayerLevelManager>
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;

    [InlineEditor]
    // Danh sách điểm kinh nghiệm cần để lên mỗi cấp
    [SerializeField] private List<ListLevelUp> m_ListlevelUp;
    private ListLevelUp m_CurrentLevelUp;
    private const string levelUpPath = "Scripts/SO/ExpData";

    //[SerializeField] private UnityEvent onLevelUp;

    private void Start()
    {
        LoadExpTable();
        m_CurrentLevelUp = m_ListlevelUp[0];
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
      if(currentExp>= m_CurrentLevelUp.expNeedLvup)
        {
            currentExp -= m_CurrentLevelUp.expNeedLvup;
            currentLevel++;
            if (currentLevel < m_ListlevelUp.Count)
            {
                m_CurrentLevelUp = m_ListlevelUp[currentLevel - 1];
            }
            else
            {
                // Đã đạt đến cấp tối đa
                currentLevel = m_ListlevelUp.Count;
                currentExp = 0;
            }
            //onLevelUp?.Invoke();
        }
    }

    private void LoadExpTable()
    {
        ListLevelUp[] listLevelUps = Resources.LoadAll<ListLevelUp>(levelUpPath);
       foreach(ListLevelUp level in listLevelUps)
        {
            m_ListlevelUp.Add(level);
        }
    }
}
