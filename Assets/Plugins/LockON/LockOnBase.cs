using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class LockOnBase : MonoBehaviour
{
    private enum TYPECAMERA
    {
        DEFAULT = 0,
        FREELOOK,
        LOCKON
    }
    [SerializeField] private CinemachineVirtualCameraBase m_LockOnCamera;
    [SerializeField] private CinemachineVirtualCameraBase m_FreeLookCamera;
    [SerializeField] private float m_Radius;
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private GameObject m_LockOnIconPrefab;
    private GameObject m_IconCurrent;
    [SerializeField] private Camera Camera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float m_ValueBoss; // 0.04
    [SerializeField] private float m_ValueCreep; // 0.15
    private CinemachineRotationComposer m_RotationComposer;
    private Transform m_Target;

    [SerializeField] private bool m_IsLockOn = false;
    private int m_TargetIndex = 0;

    [SerializeField] private List<Transform> nearbyEnemies = new List<Transform>();

    // Thêm biến cho việc cập nhật theo chu kỳ
    [SerializeField] private float updateInterval = 0.5f;
    private float updateTimer = 0f;

    private void Awake()
    {
        m_RotationComposer = m_LockOnCamera.GetComponent<CinemachineRotationComposer>();
        if(m_RotationComposer == null)
        {
            Debug.LogWarning($"Chưa có {m_RotationComposer}");
        } 
            
        Camera = Camera.main;
    }

    void Start()
    {
        m_IconCurrent = Instantiate(m_LockOnIconPrefab, canvas.transform);
        m_IconCurrent.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLockOn();
        }

        if (m_IsLockOn)
        {
            // Cập nhật danh sách enemy theo chu kỳ thay vì mỗi frame
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;
                UpdateNearbyEnemies();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ChangeLockOnTarget();
            }

            if (m_Target != null)
            {
                UpdateIconPosition();
            }
        }
    }

    private void ToggleLockOn()
    {
        m_IsLockOn = !m_IsLockOn;
        Debug.Log("LockOn: " + m_IsLockOn);
        UpdateNearbyEnemies();

        // Khi bật Lock-on, nếu có enemy trong vùng sẽ chọn mục tiêu đầu tiên
        if (m_IsLockOn && nearbyEnemies.Count > 0)
        {
            m_TargetIndex = 0;
            LockOn(nearbyEnemies[m_TargetIndex]);
            SetCameraActive(TYPECAMERA.LOCKON);
        }
        else
        {
            UnLock();
            SetCameraActive(TYPECAMERA.FREELOOK);
        }
    }

    private void ChangeLockOnTarget()
    {
        if (nearbyEnemies.Count == 0)
            return;

        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        // Duyệt qua tất cả enemy trong nearbyEnemies
        foreach (Transform enemy in nearbyEnemies)
        {
            // Ưu tiên bỏ qua mục tiêu hiện tại nếu có enemy khác
            if (enemy == m_Target && nearbyEnemies.Count > 1)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // Nếu tìm thấy enemy gần nhất khác với mục tiêu hiện tại, chuyển qua LockOn enemy đó
        if (nearestEnemy != null)
        {
            LockOn(nearestEnemy);
        }
    }


    private void LockOn(Transform newTarget)
    {
        m_Target = newTarget;

        if (m_Target != null)
        {
            m_IconCurrent.SetActive(true);
            m_LockOnCamera.LookAt = m_Target;

            if (m_RotationComposer != null)
            {
                m_RotationComposer.Composition.ScreenPosition.y = newTarget.CompareTag("Creep") ? m_ValueCreep : m_ValueBoss;
            }else
            {
                Debug.LogWarning($"chưa gắn {m_RotationComposer}");
            }    
        }
    }

    private void UnLock()
    {
        m_IconCurrent.SetActive(false);
        m_Target = null;
        m_LockOnCamera.LookAt = null;
    }

    private void UpdateIconPosition()
    {
        Vector3 chestPos = GetChestPosition(m_Target);
        Vector3 screenPos = Camera.WorldToScreenPoint(chestPos);
        m_IconCurrent.GetComponent<RectTransform>().position = screenPos;
    }

    private Vector3 GetChestPosition(Transform target)
    {
        if (target == null) return Vector3.zero;

        CapsuleCollider capsule = target.GetComponent<CapsuleCollider>();
        if (capsule != null) return target.position + Vector3.up * capsule.height * 0.4f;

        CharacterController character = target.GetComponent<CharacterController>();
        if (character != null) return character.bounds.center + Vector3.up * character.height * 0.4f;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null) return renderer.bounds.center + Vector3.up * renderer.bounds.size.y * 0.2f;

        return target.position + Vector3.up * 1f;
    }

    private void UpdateNearbyEnemies()
    {
        // Lưu lại mục tiêu hiện tại để so sánh sau khi cập nhật danh sách mới
        Transform currentTarget = m_Target;
        nearbyEnemies.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_Radius, m_LayerMask);
        foreach (Collider collider in hitColliders)
        {
            nearbyEnemies.Add(collider.transform);
        }

        if (m_IsLockOn)
        {
            // Nếu mục tiêu hiện tại vẫn có trong danh sách mới thì giữ nguyên
            if (currentTarget != null && nearbyEnemies.Contains(currentTarget))
            {
                return;
            }
            else
            {
                if (nearbyEnemies.Count > 0)
                {
                    // Chuyển sang mục tiêu đầu tiên trong danh sách mới
                    m_TargetIndex = 0;
                    LockOn(nearbyEnemies[m_TargetIndex]);
                    SetCameraActive(TYPECAMERA.LOCKON);
                }
                else
                {
                    // Không còn kẻ địch nào trong vùng
                    UnLock();
                    SetCameraActive(TYPECAMERA.FREELOOK);
                }
            }
        }
        else
        {
            UnLock();
            SetCameraActive(TYPECAMERA.FREELOOK);
        }
    }

    private void SetCameraActive(TYPECAMERA typeCamera)
    {
        if (typeCamera.Equals(TYPECAMERA.FREELOOK))
        {
            m_LockOnCamera.Priority = 10;
            m_FreeLookCamera.Priority = 20;
        }
        else if (typeCamera.Equals(TYPECAMERA.LOCKON))
        {
            m_LockOnCamera.Priority = 20;
            m_FreeLookCamera.Priority = 10;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_Radius);
    }
}
