using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;

public class SpawnAxe : MonoBehaviour
{
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private int circleCount = 16;      // số phân thân sẽ tỏa vòng tròn
    [SerializeField] private int poolCount = 16;  
    [SerializeField] private float axeSpeed = 10f;
    [SerializeField] private float zSpinSpeed = 10f;
    [SerializeField] private float SpeedAngle = 10f;
    [SerializeField] private float posY;
    [SerializeField] private Vector3 boxHalfExtents = new(1f, 1f, 1f);
    [SerializeField] private LayerMask layerMask;


    [SerializeField] private float delayBeforeCircle = 0.5f; // delay trước khi tỏa
    private readonly Queue<GameObject> pool = new();
    private readonly List<GameObject> activeAxes = new();
    public bool m_IsThrow;

    private void Start()
    {
        CreatePool();
    }
 
    private IEnumerator SpawnCircleAxes()
    {
        //// Chờ tới khi rìu gốc đủ gần player
        //while (true)
        //{
        //    Collider[] collider = Physics.OverlapBox(axePrefab.transform.position, boxHalfExtents,Quaternion.identity, layerMask);
        //    if(collider.Length >0)
        //        break;
        //    yield return null; // chờ frame tiếp
        //}

        // Lấy vị trí trung tâm từ rìu ban đầu
        //Vector3 center = axePrefab.transform.position;
        Vector3 center = transform.position + transform.up * posY;

        // Spawn circleCount rìu tỏa đều 360°
        for (int i = 0; i < circleCount; i++)
        {
            float angle = i * (360f / circleCount);
            Quaternion circleYaw = Quaternion.Euler(180, angle, 90);
            Vector3 dir = circleYaw * Vector3.forward;

            // Lấy rìu từ pool và set position, rotation
            var clone = GetPool();
            if (clone != null)
            {
                clone.transform.SetPositionAndRotation(center, circleYaw);
                clone.SetActive(true);

                activeAxes.Add(clone);
                if(clone.TryGetComponent<ParticleSystem>(out var particles))
                {
                    particles.Play();
                }
                if (clone.TryGetComponent<ChildHitBox>(out var childBoxes))
                {
                    childBoxes.TypeCollider = TypeCollider.ThrowAxeFx;
                }
                if (clone.TryGetComponent<Rigidbody>(out var rb))
                {
                    // 1) Di chuyển
                    rb.linearVelocity = dir * axeSpeed;

                    // 2) Spin: lấy trục local forward (z) của clone làm trục quay
                    Vector3 spinAxisWorld = clone.transform.up;
                    // zSpinSpeed hiện là độ/giây, angularVelocity cần radian/giây
                    float spinRad = zSpinSpeed * Mathf.Deg2Rad;
                    rb.angularVelocity = SpeedAngle * spinRad * spinAxisWorld;
                }
            }    
        }
        // 2) Chờ 0.6s SAU khi đã spawn hết
        yield return new WaitForSeconds(delayBeforeCircle);

        // 3) Return tất cả cùng lúc
        ReturnAllAxes();
    }
    private void ReturnAllAxes()
    {
        foreach (var axe in activeAxes)
        {
            if (axe.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            axe.SetActive(false);
            pool.Enqueue(axe);
        }
        activeAxes.Clear();
    }
    private void CreatePool()
    {
        for (int i = 0; i < poolCount; i++)
        {
            GameObject axe = Instantiate(axePrefab, transform);
            axe.SetActive(false);
            pool.Enqueue(axe);
        }
    }
    private GameObject GetPool()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(
            axePrefab.transform.position,
            Quaternion.identity,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
    public void StartSpinAxe()
    {
        StartCoroutine(SpawnCircleAxes());
    }
  
}
