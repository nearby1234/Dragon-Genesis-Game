using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        // Kiểm tra xem va chạm có xảy ra với đối tượng mong muốn không
        if (other.CompareTag("Player"))
        {
            
            // Gọi hàm ReturnPoolOrbEffect để trả về pool 
            ExpOrbEffectSpawner expOrbEffectSpawner = GetComponentInParent<ExpOrbEffectSpawner>();  
            expOrbEffectSpawner.ReturnPoolOrbEffect(gameObject);
            if(UIManager.HasInstance)
            {
                // gọi hàm GetObjectFormPool để tạo hiệu ứng orb di chuyển lên UI
                UIManager.Instance.SpawnObjectVFXPrefab.GetObjectFormPool();
                
            }
        }
    }

}
