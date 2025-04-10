using System;
using System.Collections;
using UnityEngine;

public class EnemyHeal : MonoBehaviour
{
    [SerializeField] private int m_EnemyHeal;
    public int GetEnemyHeal => m_EnemyHeal;
    [SerializeField] private float m_Timer;
    [SerializeField] private EnemyController m_EnemyController;
    public EnemyController GetEnemyController => m_EnemyController;
    [SerializeField] private bool m_IsDead;
    private void Awake()
    {
        m_EnemyController = GetComponent<EnemyController>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CREEP_SEND_HEAL_VALUE, IniHealValue);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CREEP_SEND_HEAL_VALUE, IniHealValue);
        }
    }
    public bool IsEnemyDead() => m_IsDead;
    public void ReducePlayerHealth(int damaged)
    {
        // Nếu enemy đã chết thì không xử lý
        if (m_IsDead) return;

        // Trừ máu
        m_EnemyHeal -= damaged;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.CREEP_UPDATE_HEAL_VALUE, (this.gameObject, m_EnemyHeal));
        }
        // Nếu máu giảm về 0 hoặc dưới 0, gọi Die() và thoát ngay
        if (m_EnemyHeal <= 0)
        {
            if (EffectManager.HasInstance)
            {
                EffectManager.Instance.ExpOrbEffectSpawner.SpawnOrbs(transform.position, 5);
            }
            Die();
            return;
        }

        // Nếu chưa chết, kích hoạt trigger "Hit"
        m_EnemyController.GetAnimator().SetTrigger("Hit");
    }

    private void Die()
    {
        m_IsDead = true;
        m_EnemyController.GetAnimator().SetTrigger("Death");
        m_EnemyController.GetCollider.enabled = false;
    }
    private void IniHealValue(object value)
    {
        if (value != null && value is (GameObject enemyObj, EnemyStatSO enemyStatSO))
        {
            // Kiểm tra nếu sự kiện dành cho enemy này
            if (enemyObj.Equals(this.gameObject))
            {
                m_EnemyHeal = enemyStatSO.heal;
            }
        }
    }

    



}
