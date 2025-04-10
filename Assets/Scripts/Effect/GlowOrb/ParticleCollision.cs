using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        // Kiểm tra xem va chạm có xảy ra với đối tượng mong muốn không
        if (other.CompareTag("Player"))
        {
            // Gọi hàm SpawnOrbs để tạo hiệu ứng văng lên
            ExpOrbEffectSpawner expOrbEffectSpawner = GetComponentInParent<ExpOrbEffectSpawner>();  
            expOrbEffectSpawner.ReturnPoolOrbEffect(gameObject);
            if(UIManager.HasInstance)
            {
                UIManager.Instance.SpawnObjectVFXPrefab.GetObjectFormPool();
            }
        }
    }

}
