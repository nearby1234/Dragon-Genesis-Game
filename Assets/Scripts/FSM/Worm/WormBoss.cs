using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WormBoss : BaseBoss<WormBoss, WORMSTATE>
{
    [Header("Worm Imformation")]
    public EnemyStatSO WormAttributeSO;
    public float m_WormBossHeal;
    public bool m_IsGetDamage;
    public bool isInHitState = false;
    public bool idleGraceActive = false;
    public bool IsRageState = false;
    public bool isShowHealbarBoss = false;

    [Header("Atribute")]
    public float m_AngularSpeed;
    public int currentAttackIndex = 0;
    private Vector3 center;
    private Vector3 size;
    public float m_StopDistance;

    [Header("Detection & Timing")]
    public float detectionRange = 10f;
    public float undergroundDuration = 3f;

    [Header("Attack Settings")]
    public List<WormAttackData> wormAttackDatasPhase1 = new();
    public List<WormAttackData> wormAttackDatasPhase2 = new();

    [Header("Animations")]
    public Animator Animator => animator;
    public string undergroundAnimation = "GroundDiveIn";
    public string emergeAnimation = "GroundBreakThrough";

    [Header("Material")]
    [SerializeField] private Material m_DefaultMaterial;
    [SerializeField] private Material m_GetHitMaterial;
     

    [Header("References")]
    public string[] listStringRefer;
    public GameObject m_Player;
    [SerializeField] private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    public NavMeshAgent NavMeshAgent => m_NavmeshAgent;
    public DissovleController dissovleController;
    private void Awake()
    {
        m_NavmeshSurface = GameObject.Find(listStringRefer[0]).GetComponent<NavMeshSurface>();
        m_Player = GameObject.Find(listStringRefer[1]);
        animator = GetComponent<Animator>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        dissovleController = GetComponent<DissovleController>();
        m_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    protected override void Start()
    {
        m_WormBossHeal = WormAttributeSO.heal;
        finiteSM = new FSM<WormBoss, WORMSTATE>();
        finiteSM.ChangeState(new WormIdleState(this, finiteSM));
        if (m_NavmeshSurface != null)
        {
            size = m_NavmeshSurface.size;
            center = m_NavmeshSurface.transform.position + m_NavmeshSurface.center;
        }
        
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.BOSS_SEND_HEAL_VALUE, m_WormBossHeal);
        }
    }
    protected override void Update()
    {
        finiteSM?.Update();
        if (PlayerInRange())
        {
            if(!isShowHealbarBoss)
            {
                UIManager.Instance.ShowScreen<ScreenHealBarBoss>();
                isShowHealbarBoss = true;
            }    
        }
        
        
    }
    public override void RequestStateTransition(WORMSTATE requestedState)
    {
        switch (requestedState)
        {
            case WORMSTATE.IDLE:
                finiteSM.ChangeState(new WormIdleState(this, finiteSM));
                break;
            case WORMSTATE.WALK:
                finiteSM.ChangeState(new WormWalkState(this, finiteSM));
                break;
            case WORMSTATE.UNDERGROUND:
                finiteSM.ChangeState(new WormUndergroundState(this, finiteSM));
                break;
            case WORMSTATE.EMERGE:
                finiteSM.ChangeState(new WormEmergeState(this, finiteSM));
                break;
            case WORMSTATE.DETEC:
                finiteSM.ChangeState(new WormDetecState(this, finiteSM));
                break;
            case WORMSTATE.ATTACK:
                finiteSM.ChangeState(new WormAttackState(this, finiteSM));
                break;
            case WORMSTATE.HIT:
                finiteSM.ChangeState(new WormHitState(this, finiteSM));
                break;
            case WORMSTATE.RAGE:
                finiteSM.ChangeState(new WormRageState(this, finiteSM));
                break;
            case WORMSTATE.DIE:
                finiteSM.ChangeState(new WormDieState(this, finiteSM));
                break;
            default:
                Debug.LogWarning($"State {requestedState} chưa được cài đặt trong RequestStateTransition.");
                break;
        }
    }
    public Vector3 GetRandomEmergencePosition()
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        return new Vector3(x, y, z);
    }
    public void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomEmergencePosition();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            m_NavmeshAgent.stoppingDistance = 0;
            m_NavmeshAgent.Warp(hit.position);
        }
        else
        {
            Debug.Log("ko tim thay duong di");
        }
    }
    public void ChangeBeforeState(WORMSTATE wormState)
    {
        beforeState = wormState;
    }
    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, m_Player.transform.position) <= detectionRange;
    }
    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, m_Player.transform.position) <= m_StopDistance;
    }
    public Vector3 DistanceToPlayerNormalized()
    {
        return (m_Player.transform.position - transform.position).normalized;
    }
    public void Rotation()
    {
        Vector3 directionToPlayer = DistanceToPlayerNormalized();
        directionToPlayer.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = m_AngularSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    public int GetRandomIndexAttackList()
    {
        // Nếu boss đã vào rage, sử dụng danh sách phase2, ngược lại phase1
        List<WormAttackData> attackList = IsRageState ? wormAttackDatasPhase2 : wormAttackDatasPhase1;
        int indexCurrentAttack = Random.Range(0, attackList.Count);
        return indexCurrentAttack;
    }


    public void GetDamage(int damage)
    {
        // Nếu máu boss đã ≤ 0 thì boss chết.
        if (m_WormBossHeal <= 0)
        {
            IsRageState = false;
            //RequestStateTransition(WORMSTATE.DIE);
            Debug.Log("Worm Boss Die");
            return;
        }

        // Nếu boss đã ở phase 2 (RAGE), chỉ trừ damage mà không chuyển state.
        if (IsRageState)
        {
            if (!TryApplyDamage(damage))
                return;
            return;
        }

        // Nếu sau khi trừ damage, máu boss ≤ 50 và boss chưa ở state RAGE,
        // chuyển ngay sang state RAGE (phase 2) và áp dụng damage.
        if (m_WormBossHeal - damage <= 50f && !currentState.Equals(WORMSTATE.RAGE))
        {
            if (!TryApplyDamage(damage))
                return;
            IsRageState = true;
            RequestStateTransition(WORMSTATE.RAGE);
            return;
        }

        // Nếu boss đang ở state UNDERGROUND và đang trong khoảng thời gian grace,
        // chỉ trừ damage mà không chuyển state.
        if (currentState.Equals(WORMSTATE.UNDERGROUND) && idleGraceActive)
        {
            if (!TryApplyDamage(damage))
                return;
            return;
        }

        // Nếu boss đã ở state HIT, chỉ trừ damage.
        if (isInHitState)
        {
            if (!TryApplyDamage(damage))
                return;
            return;
        }
        else
        {
            // Trong trường hợp khác, boss chưa ở state HIT,
            // áp dụng damage và chuyển sang state HIT.
            if (!TryApplyDamage(damage))
                return;
            RequestStateTransition(WORMSTATE.HIT);
            return;
        }
    }

    private bool TryApplyDamage(float damage)
    {
        // Nếu damage đã được áp dụng cho va chạm hiện tại, trả về false.
        if (m_IsGetDamage)
            return false;

        // Trừ damage, đặt flag và khởi chạy coroutine reset flag.
        m_WormBossHeal -= damage;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.BOSS_UPDATE_HEAL_VALUE, m_WormBossHeal);
        }
        m_SkinnedMeshRenderer.material = m_GetHitMaterial;
        StartCoroutine(RestoreDefaultMaterial(0.2f));

        if(m_WormBossHeal <= 0)
        {
            IsRageState = false;
            RequestStateTransition(WORMSTATE.DIE);
            return true;
        }    
        m_IsGetDamage = true;
        StartCoroutine(ResetDamageFlag());
        return true;
    }
    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(0.5f);
        m_IsGetDamage = false;
    }
    private IEnumerator RestoreDefaultMaterial(float timer)
    {
        yield return new WaitForSeconds(timer);
        m_SkinnedMeshRenderer.material = m_DefaultMaterial;
    }

   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_StopDistance);

    }
}
