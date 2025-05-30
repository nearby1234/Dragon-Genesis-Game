using Sirenix.OdinInspector;
using UnityEngine;


public enum TypeCollider
{
    UnKnown = 0,
    Leg,
    Axe,
    AxeFX,
}
public enum BullTankState
{
    UnKnown = 0,
    Default,
    Jump,
    Attack,
}

public class BullTankDamage : MonoBehaviour
{
    [InlineEditor]
    [SerializeField] private BehaviorTreeSO bullTankSetting;
    [SerializeField] private float DamageBase;
    [SerializeField] private BullTankState bullTankState;
    private bool IsAttackJump;
    private bool IsAttackAxe;

    private void Start()
    {
        DamageBase = bullTankSetting.DamageBase;
        bullTankState = BullTankState.Default;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.TYPECOLLIDER_CHILD, OnChildHit);
        }
    }

    public void OnChildHit(object value)
    {
        if (value != null)
        {
            if (value is (TypeCollider typeCollider, Collider other))
            {
                PlayerHeal playerHeal = other.GetComponent<PlayerHeal>();
                switch (typeCollider)
                {
                    case TypeCollider.Leg:
                        {
                            float damageLeg = CaculateDamage(bullTankSetting.percentAttackJump);
                            
                            if (playerHeal != null && bullTankState == BullTankState.Jump)
                            {
                                if (!IsAttackJump)
                                {
                                    playerHeal.ReducePlayerHeal((int)damageLeg, TypeCollider.Leg);
                                    PlayFxBlood(other);
                                    if (AudioManager.HasInstance) AudioManager.Instance.PlaySE("PlayerHit");
                                    IsAttackJump = true;
                                }
                            }
                        }
                        break;
                    case TypeCollider.Axe:
                        {
                            float damageAxe = CaculateDamage(bullTankSetting.percentAttackAxe);
                            if (playerHeal != null && bullTankState == BullTankState.Attack)
                            {
                                if (!IsAttackAxe)
                                {
                                    playerHeal.ReducePlayerHeal((int)damageAxe, TypeCollider.Axe);
                                    PlayFxBlood(other);
                                    if (AudioManager.HasInstance) AudioManager.Instance.PlaySE("PlayerHit");
                                    IsAttackAxe = true;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private float CaculateDamage(float percentDamage)
    {
        float result = DamageBase * (percentDamage / 100f);
        return result;
    }

    public void SetState(BullTankState bullTankState)
    {
        this.bullTankState = bullTankState;
    }
   

    public void SetStateAttackJumpFlag() => IsAttackJump = false;
    public void SetStateAttackAxeFlag() => IsAttackAxe = false;
  
    private void PlayFxBlood(Collider collider)
    {
        GameObject bloodPrefabs;
        if (EffectManager.HasInstance)
        {
            bloodPrefabs = EffectManager.Instance.GetPrefabs("BloodFx");
            Vector3 closePoint = collider.ClosestPoint(transform.position);
            GameObject obj = Instantiate(bloodPrefabs, closePoint, Quaternion.identity);
            Destroy(obj, 2f);
        }
    }
}
