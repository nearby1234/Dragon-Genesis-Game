using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using System.Collections;

public class PlayerLevelManager : BaseManager<PlayerLevelManager>
{
    private int totalStatPoints = 0;
    public int TotalStatPoints => totalStatPoints;
    [SerializeField] private int currentLevel = 1;
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            int delta = value - currentLevel;
            currentLevel = value;
            if (delta > 0)
            {
                totalStatPoints += 5 * delta;
                if (ListenerManager.HasInstance)
                    ListenerManager.Instance.BroadCast(ListenType.UI_SEND_VALUE_LEVEL, totalStatPoints);
                    ListenerManager.Instance.BroadCast(ListenType.UI_SEND_LEVEL_PLAYER, currentLevel);
            }
        }
    }    
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expNextLevel;
    [SerializeField] private GameObject m_LevelUpFx;
    public int CurrentExp => currentExp;
    public int ExpNextLevel => expNextLevel; // giá trị exp cần để lên cấp tiếp theo
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
   
    [SerializeField] private int m_ExpOrbValue = 20; // Giá trị exp của orb
    public int ExpOrbValue => m_ExpOrbValue; // Giá trị exp của orb

    //[SerializeField] private UnityEvent onLevelUp;

    protected override void Awake()
    {
        base.Awake();
        LoadExpTable();
        if (m_ListlevelUp.Count > 0)
            m_CurrentLevelUp = m_ListlevelUp[0];
    }
    private void Start()
    {
        expNextLevel = m_CurrentLevelUp.expNeedLvup;
        m_LevelUpFx = EffectManager.Instance.GetPrefabs("Lvlup"); // Prefab dạng GameObject
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
    public void UpdateTotalPoint(int valuePoint)
    {
        totalStatPoints = valuePoint;
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
      while(currentExp>= m_CurrentLevelUp.expNeedLvup)
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
                break;
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
        Destroy(fxInstance, 1f);
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
