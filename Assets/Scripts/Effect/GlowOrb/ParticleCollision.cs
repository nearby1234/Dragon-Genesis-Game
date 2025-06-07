using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField] private float autoReturnDelay = 2f;      // Thời gian đợi trước khi tự động return
    private Coroutine _autoReturnCoroutine;
    [SerializeField] private Transform player;

    private void OnEnable()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_POS, ReceiverPlayerPosition);
        }
        // Khi orb vừa active, bắt đầu đếm ngược để tự động return
        _autoReturnCoroutine = StartCoroutine(AutoReturnCoroutine());
    }

    private void OnDisable()
    {
       
        // Dọn dẹp coroutine nếu object bị deactivate/destroy
        if (_autoReturnCoroutine != null)
            StopCoroutine(_autoReturnCoroutine);
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_POS, ReceiverPlayerPosition);
        }
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
        if(player !=null)
        {
            transform.DOMove(player.position, 1f).SetEase(Ease.InBack)
          .OnComplete(() =>
          {
              // 1) trả orb về pool
              var spawner = GetComponentInParent<ExpOrbEffectSpawner>();
              spawner.ReturnPoolOrbEffect(gameObject);
              // 2) spawn VFX trên UI
              if (UIManager.HasInstance)
                  UIManager.Instance.SpawnObjectVFXPrefab.GetObjectFormPool(1);
          });
        }
    }
    private void ReceiverPlayerPosition(object value)
    {
        if (value is Transform playerTransform)
        {
            Debug.Log("Received player position from ListenerManager");
            player = playerTransform;
        }
    }
}
