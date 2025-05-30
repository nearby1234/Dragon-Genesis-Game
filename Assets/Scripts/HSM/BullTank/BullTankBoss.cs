using System;
using System.Collections;
using System.Drawing;
using System.Net.Sockets;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public enum StateHSM
{
    Unknow,
    Idle,
    Attack,
    Combo,
}

public class BullTankBoss : StateMachine<BullTankBoss>
{
    [Header("States")]
    public string currentSubState;
    public string currentSuperState;

    [Header("Setting Draw Gizmos")]
    public float m_ZoneDetecPlayerDraw;
    public float m_ZoneAttackPlayerDraw;

    [Header("Reference")]
    [SerializeField] private GameObject m_Player;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private NavMeshSurface m_BullTankSurface;
    [SerializeField] private NavMeshAgent m_BullTankAgent;
    [SerializeField] private EnemyStatSO m_BullTankSO;

    [Header("Setting BullTank")]
    public float m_DistanceAttackJump;
    public float m_DistanceAttackSword;
    public float m_SpeedWalk;
    public float m_TimeTranferComboState;

    [Header("Debug/Test")]
    public bool debugMode = false;
    public StateHSM debugStateName;


    // Property
    public Animator Animator => m_Animator;
    public NavMeshAgent BullTankAgent => m_BullTankAgent;
    public GameObject Player => m_Player;

    private void Start()
    {
        var superIdleState = new SuperIdleState(this);
        var superAttackState = new SuperAttackStateBT(this);
        var superComboState = new SuperComboState(this);
        
        superIdleState.OnStateComplete += (state) => SetState(superAttackState);
        superAttackState.OnStateComplete += (state) => SetState(superComboState);
        m_DistanceAttackJump = m_BullTankSO.distanceAttackJump;
        m_DistanceAttackSword = m_BullTankSO.distanceAttackSword;
        m_SpeedWalk = m_BullTankSO.speedWalk;
        DebugTest();
        SetState(superIdleState);
    }
    protected override void Update()
    {
        //if(m_HasCompleteSuperIdleState && !m_HasCompleteSuperAttackState && IsWithin(m_ZoneDetecPlayerDraw))
        //{
        //    SetState(new SuperAttackStateBT(this));
        //}
        //if(m_HasCompleteSuperAttackState)
        //{
        //    if (!m_IsTranferComboState)
        //    {
        //        m_Animator.SetTrigger("Idle");
        //        StartCoroutine(WaitTranferComboState());
        //        return;
        //    }
        //}    
        base.Update();
    }

    public void DebugTest()
    {
        if (debugMode && debugStateName != StateHSM.Unknow)
        {
            switch (debugStateName)
            {
                case StateHSM.Combo:
                    SetState(new SuperComboState(this));
                    break;
                case StateHSM.Attack:
                    SetState(new SuperAttackStateBT(this));
                    break;
                default:
                    Debug.LogWarning($"Không có State {debugStateName} này");
                    break;
            }
            return;
        }
       
    }
    public Vector3 GetRandomEmergencePosition()
    {
        // Lấy center của surface sang world-space
        Vector3 worldCenter = m_BullTankSurface.transform.TransformPoint(m_BullTankSurface.center);
        Vector3 size = m_BullTankSurface.size;

        float x = Random.Range(worldCenter.x - size.x / 2, worldCenter.x + size.x / 2);
        float y = worldCenter.y;
        float z = Random.Range(worldCenter.z - size.z / 2, worldCenter.z + size.z / 2);
        return new Vector3(x, y, z);
    }
    public bool MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomEmergencePosition();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            m_BullTankAgent.isStopped = false;
            m_BullTankAgent.stoppingDistance = 2f;
            m_BullTankAgent.SetDestination(hit.position);
            Debug.Log($"position :  {hit.position}");
            return true;
        }
        else
        {
            Debug.Log($"không tìm thấy {hit.position}");
            return false;
        }
    }

    public bool IsWithin(float radius) => GetDistanceToPlayer() <= radius;
    public float GetDistanceToPlayer() => Vector3.Distance(Player.transform.position, transform.position);

    public bool HasArrived()
    {
        // 1. Chưa tính xong đường thì chưa xét tiếp
        if (m_BullTankAgent.pathPending)
            return false;

        // 2. Path phải được tính xong và thành công
        if (m_BullTankAgent.pathStatus != NavMeshPathStatus.PathComplete)
            return false;

        // 3. Agent phải đang có path để theo
        if (!m_BullTankAgent.hasPath)
            return false;

        // 4. Khoảng cách còn lại phải <= stoppingDistance
        if (m_BullTankAgent.remainingDistance > m_BullTankAgent.stoppingDistance)
            return false;

        // 5. Và agent phải thực sự dừng (velocity gần 0)
        if (m_BullTankAgent.velocity.sqrMagnitude > 0.1f)
            return false;

        return true;
    }

    public bool HasStopDistance()
    {
        return m_BullTankAgent.remainingDistance <= m_BullTankAgent.stoppingDistance;
    }
    public bool IsRangeAttackPlayer(float range)
    {
        m_BullTankAgent.stoppingDistance = range;
        return m_BullTankAgent.remainingDistance <= m_BullTankAgent.stoppingDistance;
    }
    public float Rotation()
    {
        // Tính Hướng
        Vector3 directionToPlayer = (m_Player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0f;
        // Tính Quaternion mục tiêu
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = m_BullTankAgent.angularSpeed;
        // Thực hiện xoay dần
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        // Tính góc chênh lệch sau khi xoay
        float angel = Quaternion.Angle(transform.rotation, targetRotation);
        return angel;
    }
    public bool RotateForwardPlayer()
    {
        return Rotation() <= 2f;
    }

    public void EnableAgentRotation(bool v) => BullTankAgent.updateRotation = v;
    public void EnableAgentPosition(bool v) => BullTankAgent.updatePosition = v;
    public void SetSubStateHSM(object state)
    {
        string name = state.GetType().Name;
        if (name != null)
        {
            currentSubState = name;
        }
    }
    public void SetSuperStateHSM(object state)
    {
        string name = state.GetType().Name;
        if (name != null)
        {
            currentSuperState = name;
        }
    }
    public void HandleAnimationComplete(NameState NameState)
    {
        CurrentState?.OnAnimationComplete(NameState);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, m_ZoneDetecPlayerDraw);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_ZoneAttackPlayerDraw);
    }
}
