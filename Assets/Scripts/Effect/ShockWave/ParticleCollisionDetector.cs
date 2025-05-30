using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionDetector : MonoBehaviour
{
    public EnemySkill enemySkill; // Tham chi?u ??n script qu?n lý

    private void Awake()
    {
        enemySkill = GetComponentInParent<EnemySkill>();
    }
    void OnParticleCollision(GameObject other)
    {
        // L?y ParticleSystem hi?n hành
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // L?y thông tin chi ti?t c?a va ch?m n?u c?n
        List<ParticleCollisionEvent> collisionEvents = new();
        _ = ParticlePhysicsExtensions.GetCollisionEvents(ps, other, collisionEvents);

        // G?i hàm x? lý trong EnemySkill
        if (enemySkill != null)
        {
            enemySkill.HandleParticleCollision( other, collisionEvents);
        }
    }

}
