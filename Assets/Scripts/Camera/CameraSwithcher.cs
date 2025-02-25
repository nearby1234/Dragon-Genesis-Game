using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase m_MainCam;
    [SerializeField] private CinemachineVirtualCameraBase m_LockOnCamera;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private float m_LockOnRange = 10f;
    [SerializeField] private LayerMask m_Enemylayer;
    [SerializeField] private GameObject m_LockOnIndicatorPrefabs;

    private GameObject m_CurrentTarget;
    private GameObject m_LockonIndicator;
    private bool m_IsLockedOn = false;
    private bool m_IsAutoLockOnEnabled = false;
    private Coroutine m_LockOnCheckCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_IsAutoLockOnEnabled = !m_IsAutoLockOnEnabled;
            Debug.Log("Auto Lock-On: " + (m_IsAutoLockOnEnabled ? "Bật" : "Tắt"));

            if (!m_IsAutoLockOnEnabled)
            {
                UnLock();
            }
        }

        if (m_IsAutoLockOnEnabled)
        {
            if (m_CurrentTarget == null)
            {
                GameObject foundEnemy = FindClosestEnemy();
                if (foundEnemy != null)
                {
                    LockOn(foundEnemy);
                }
            }
            else
            {
                float distance = Vector3.Distance(m_Player.transform.position, m_CurrentTarget.transform.position);
                if (distance > m_LockOnRange)
                {
                    if (m_LockOnCheckCoroutine == null)
                    {
                        m_LockOnCheckCoroutine = StartCoroutine(CheckEnemyReentry());
                    }
                }
            }
        }

        if (m_IsLockedOn && m_CurrentTarget != null)
        {
            if (m_LockonIndicator != null)
            {
                m_LockonIndicator.transform.position = m_CurrentTarget.transform.position + Vector3.up * 2f;
            }
        }
    }

    private IEnumerator CheckEnemyReentry()
    {
        float checkTime = 1.5f; // Chờ 1.5 giây xem enemy có quay lại không
        float elapsedTime = 0f;

        while (elapsedTime < checkTime)
        {
            GameObject foundEnemy = FindClosestEnemy();
            if (foundEnemy == m_CurrentTarget)
            {
                Debug.Log("Enemy đã quay lại, giữ Lock-On");
                m_LockOnCheckCoroutine = null;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Enemy đã rời xa, hủy Lock-On");
        UnLock();
        m_LockOnCheckCoroutine = null;
    }

    private void LockOn(GameObject enemy)
    {
        if (m_IsLockedOn) return;

        m_CurrentTarget = enemy;
        m_IsLockedOn = true;

        m_MainCam.gameObject.SetActive(false);
        m_LockOnCamera.gameObject.SetActive(true);

        m_LockOnCamera.Follow = m_Player.transform;
        m_LockOnCamera.LookAt = m_CurrentTarget.transform;

        if (m_LockonIndicator == null)
        {
            m_LockonIndicator = Instantiate(m_LockOnIndicatorPrefabs);
            m_LockonIndicator.transform.SetParent(transform);
        }
        m_LockonIndicator.transform.position = m_CurrentTarget.transform.position + Vector3.up * 2f;

        Debug.Log("Đã khóa mục tiêu: " + m_CurrentTarget.name);
    }

    private void UnLock()
    {
        if (!m_IsLockedOn) return;

        m_IsLockedOn = false;

        m_MainCam.gameObject.SetActive(true);
        m_LockOnCamera.gameObject.SetActive(false);

        if (m_LockonIndicator != null)
        {
            Destroy(m_LockonIndicator);
            m_LockonIndicator = null;
        }

        m_LockOnCamera.LookAt = null;
        m_LockOnCamera.Follow = null;
        m_CurrentTarget = null;

        Debug.Log("Lock-On đã hủy.");
    }

    private GameObject FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_Player.transform.position, m_LockOnRange, m_Enemylayer);
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            float distance = Vector3.Distance(m_Player.transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = collider.gameObject;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_Player.transform.position, m_LockOnRange);
    }
}
