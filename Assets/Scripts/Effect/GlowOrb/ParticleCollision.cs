using System.Collections;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField] private float autoReturnDelay = 2f;      // Thời gian đợi trước khi tự động return
    private Coroutine _autoReturnCoroutine;

    private void OnEnable()
    {
        // Khi orb vừa active, bắt đầu đếm ngược để tự động return
        _autoReturnCoroutine = StartCoroutine(AutoReturnCoroutine());
    }

    private void OnDisable()
    {
        // Dọn dẹp coroutine nếu object bị deactivate/destroy
        if (_autoReturnCoroutine != null)
            StopCoroutine(_autoReturnCoroutine);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            // Va chạm với Player: hủy timer và return ngay
            if (_autoReturnCoroutine != null)
                StopCoroutine(_autoReturnCoroutine);

            ReturnAndSpawnUI();
        }
        // Không cần else ở đây nữa, vì timer tự lo việc return đối với các trường hợp khác
    }

    private IEnumerator AutoReturnCoroutine()
    {
        // Chờ autoReturnDelay giây
        yield return new WaitForSeconds(autoReturnDelay);

        // Nếu đến đây, nghĩa là không va chạm với Player trong thời gian
        ReturnAndSpawnUI();
    }

    private void ReturnAndSpawnUI()
    {
        // 1) trả orb về pool
        var spawner = GetComponentInParent<ExpOrbEffectSpawner>();
        spawner.ReturnPoolOrbEffect(gameObject);

        // 2) spawn VFX trên UI
        if (UIManager.HasInstance)
            UIManager.Instance.SpawnObjectVFXPrefab.GetObjectFormPool(1);
    }
}
