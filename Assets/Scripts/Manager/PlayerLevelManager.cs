using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using System.Collections;

public class PlayerLevelManager : BaseManager<PlayerLevelManager>
{
    [SerializeField] private int currentLevel = 1;
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            if(ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.UI_SEND_VALUE_LEVEL, currentLevel); // Gửi thông báo đến UI về cấp độ hiện tại
            }
        }
    }    
    [SerializeField] private int currentExp = 0;
    //[SerializeField] private int expNextLevel;
    [SerializeField] private GameObject m_LevelUpFx;
    [SerializeField] private ParticleSystem m_LevelUpFxinstan;
    public int CurrentExp => currentExp;
    private float m_DisPlayExp;
    public float DisPlayExp => m_DisPlayExp; // giá trị exp hiển thị trên UI
    private Coroutine expLerpRoutine;
    [SerializeField] private float expLerpSpeed = 200f; // tốc độ cộng EXP mượt

    [InlineEditor]
    // Danh sách điểm kinh nghiệm cần để lên mỗi cấp
    [SerializeField] private List<ListLevelUp> m_ListlevelUp;
    private ListLevelUp m_CurrentLevelUp;
    public ListLevelUp CurrentLevelUp => m_CurrentLevelUp;
    private const string levelUpPath = "Scripts/SO/ExpData";
   

    //private int m_OrbCount = 0; // Số lượng orb đã thu thập
    //public int OrbCount
    //{
    //    get => m_OrbCount;
    //    set => m_OrbCount = value;
    //}
    private int m_ExpOrbValue = 20; // Giá trị exp của orb
    public int ExpOrbValue => m_ExpOrbValue; // Giá trị exp của orb

    //[SerializeField] private UnityEvent onLevelUp;

    private void Start()
    {
        LoadExpTable();
        m_CurrentLevelUp = m_ListlevelUp[0];
        //expNextLevel = m_CurrentLevelUp.expNeedLvup;
        m_LevelUpFx = EffectManager.Instance.GetPrefabs("Lvlup"); // Prefab dạng GameObject
    }
    private void Update()
    {
        //expNextLevel = m_CurrentLevelUp.expNeedLvup;
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        // Nếu đang có coroutine đang chạy thì dừng lại
        if (expLerpRoutine != null)
        {
            StopCoroutine(expLerpRoutine);
        }

        expLerpRoutine = StartCoroutine(SmoothExpIncrease());

    }
    private IEnumerator SmoothExpIncrease()
    {
        while (m_DisPlayExp < currentExp)
        {
            m_DisPlayExp = Mathf.MoveTowards(m_DisPlayExp, currentExp, expLerpSpeed * Time.deltaTime);

            //// Cập nhật UI nếu có (ví dụ thanh Exp)
            //UpdateExpBar(m_DisPlayExp);

            yield return null;
        }

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
      if(currentExp>= m_CurrentLevelUp.expNeedLvup)
        {
            currentExp -= m_CurrentLevelUp.expNeedLvup;
            m_DisPlayExp = currentExp;

            CurrentLevel++;
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
            LevelUpFX();
        }
    }
    private void LevelUpFX()
    {
        Transform transform = PlayerManager.instance.transform;

        // Instantiate bản sao prefab FX
        GameObject fxInstance = Instantiate(m_LevelUpFx, transform.position, Quaternion.identity, transform);

        // Lấy ParticleSystem từ bản vừa Instantiate
        ParticleSystem fx = fxInstance.GetComponent<ParticleSystem>();

        if (fx != null)
        {
            fx.Play();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy ParticleSystem trong prefab FX!");
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
