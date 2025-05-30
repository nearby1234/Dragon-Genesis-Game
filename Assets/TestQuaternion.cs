using UnityEngine;

public class TestQuaternion : MonoBehaviour
{
    [SerializeField] private Vector3 angelRotate;
    [SerializeField] private GameObject previewAxe;
    [SerializeField] private Transform player;        // Tham chi?u ??n Player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePreview();
    }
    void UpdatePreview()
    {
        if (previewAxe == null || player == null) return;

        // 1) Tính spawnPos, dir, lookRot
        //Vector3 spawnPos = m_ParentAxe.position;
        //Vector3 dir = (player.position - spawnPos).normalized;
        //Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion modelOffset = Quaternion.Euler(angelRotate);

        // 2) Áp rotation & position cho preview
        //previewAxe.transform.position = spawnPos;
        previewAxe.transform.rotation = modelOffset;
        previewAxe.transform.localScale = Vector3.one; // ví d?
    }
}
