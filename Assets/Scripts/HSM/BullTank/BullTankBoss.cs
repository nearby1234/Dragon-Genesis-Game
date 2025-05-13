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
    Idle,
}

public class BullTankBoss : StateMachine<BullTankBoss>
{
    [Header("States")]
    public string currentSubState;
    public string currentSuperState;

    [Header("Setting Draw Gizmos")]
    [SerializeField] private float m_ZoneDetecPlayerDraw;
    [SerializeField] private float m_ZoneAttackPlayerDraw;

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

    [Header("Flag Check")]
    [SerializeField] private bool m_HasCompleteSuperIdleState;
    [SerializeField] private bool m_HasCompleteSuperAttackState;
    [SerializeField] private bool m_IsTranferComboState;

    // Property
    public Animator Animator => m_Animator;
    public NavMeshAgent BullTankAgent => m_BullTankAgent;
    public GameObject Player => m_Player;

    [Header("Parent State")]
    private SuperIdleState idleStateComp;
    private SuperAttackStateBT attackStateComp;
    private SuperComboState comboStateComp;
    private void Start()
    {
        idleStateComp = new SuperIdleState(this);
        attackStateComp = new SuperAttackStateBT(this);
        comboStateComp = new SuperComboState(this);
        SetState(idleStateComp);
        m_DistanceAttackJump = m_BullTankSO.distanceAttackJump;
        m_DistanceAttackSword = m_BullTankSO.distanceAttackSword;
        m_SpeedWalk = m_BullTankSO.speedWalk;
    }
    protected override void Update()
    {
        if(m_HasCompleteSuperIdleState && !m_HasCompleteSuperAttackState && Distance())
        {
            SetState(attackStateComp);
        }
        if(m_HasCompleteSuperAttackState)
        {
            if (!m_IsTranferComboState)
            {
                m_Animator.SetTrigger("Idle");
                StartCoroutine(WaitTranferComboState());
                return;
            }
        }    
        base.Update();
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

    public bool Distance()
    {
        float distance = Vector3.Distance(m_Player.transform.position, this.transform.position);
        return (distance < m_ZoneDetecPlayerDraw);
    }
    public float DistanceWithPlayer()
    {
        float distance = Vector3.Distance(m_Player.transform.position, this.transform.position);
        return distance;
    }
    public bool IsMoveWayPoint()
    {
        return (
               !m_BullTankAgent.pathPending &&
                m_BullTankAgent.remainingDistance <= m_BullTankAgent.stoppingDistance &&
                m_BullTankAgent.velocity.sqrMagnitude < 0.1f);
    }
    public bool IsRangeAttackPlayer(float range)
    {
        m_BullTankAgent.stoppingDistance = range;
        return m_BullTankAgent.remainingDistance <= m_BullTankAgent.stoppingDistance;
    }
    public float Rotation()
    {
        // Tính Hướng
        Vector3 directionToPlayer = (m_Player.transform.position-transform.position).normalized;
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
    public void SetStateNavmeshAgent(int index = 0,bool IsUpdateRotation = false ,  bool IsUPdatePosition = false)
    {
        switch(index)
        {
            case 1:
                BullTankAgent.updateRotation = IsUpdateRotation;
                break;
            case 2:
                BullTankAgent.updatePosition = IsUPdatePosition;
                break;
            case 3:
                BullTankAgent.updatePosition = IsUPdatePosition;
                BullTankAgent.updateRotation = IsUpdateRotation;
                break;
            default:
                break;
        }
      
        
    }
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
    public void NotifySuperIdleStateComplete() => m_HasCompleteSuperIdleState = true;
    public void NotifySuperAttackStateComplete() => m_HasCompleteSuperAttackState = true;
    private IEnumerator WaitTranferComboState()
    {
        yield return new WaitForSeconds(m_TimeTranferComboState);
        SetState(comboStateComp);
        m_IsTranferComboState = true;
    }    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, m_ZoneDetecPlayerDraw);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_ZoneAttackPlayerDraw);
    }
}
