using Sirenix.OdinInspector;
using UnityEngine;


public enum TypeCollider
{
    UnKnown = 0,
    Leg,
    Axe,
    AxeFX,
    ThrowAxe,
    ThrowAxeFx,
    JumpFx,
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
    [SerializeField] private ChildHitBox throwAxeHitBox;
    private bool IsAttackJump;
    private bool IsAttackAxe;

    private void Start()
    {
        DamageBase = bullTankSetting.DamageBase;
        bullTankState = BullTankState.Default;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.TYPECOLLIDER_CHILD, OnChildHit);
            ListenerManager.Instance.Register(ListenType.PARTICLE_TRIGGER, OnPartilceHit);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.TYPECOLLIDER_CHILD, OnChildHit);
            ListenerManager.Instance.Unregister(ListenType.PARTICLE_TRIGGER, OnPartilceHit);
        }
    }

    private void OnChildHit(object value)
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
                                    HandlerDamageTypeCollider(playerHeal, damageLeg, TypeCollider.Leg, other, "PlayerHit");
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
                                    HandlerDamageTypeCollider(playerHeal, damageAxe, TypeCollider.Axe, other, "PlayerHit");
                                    IsAttackAxe = true;
                                }
                            }
                        }
                        break;
                    case TypeCollider.ThrowAxe:
                        {
                            float damageAxe = CaculateDamage(bullTankSetting.percentAttackAxe);
                            if (playerHeal != null && bullTankState == BullTankState.Attack)
                            {
                                if (!IsAttackAxe)
                                {
                                    HandlerDamageTypeCollider(playerHeal, damageAxe, TypeCollider.ThrowAxe, other, "PlayerHit");
                                    if(AudioManager.HasInstance)
                                    {
                                        AudioManager.Instance.PlaySE("ThrowAxeHit");
                                    }
                                    IsAttackAxe = true;

                                }
                            }
                        }
                        break;
                    case TypeCollider.ThrowAxeFx:
                        {
                            IsAttackAxe = false;
                            float damageAxe = CaculateDamage(bullTankSetting.percentAttackAxe);
                            if (playerHeal != null)
                            {
                                if (!IsAttackAxe)
                                {
                                    HandlerDamageTypeCollider(playerHeal, damageAxe, TypeCollider.ThrowAxeFx, other, "PlayerHit");
                                    if (AudioManager.HasInstance)
                                    {
                                        AudioManager.Instance.PlaySE("ThrowAxeHit");
                                    }
                                    IsAttackAxe = true;
                                }
                            }
                        }
                        break;

                }
            }
        }
    }
    private void OnPartilceHit(object value)
    {
        if (value !=null)
        {
            if (value is (TypeCollider typeCollider,GameObject other))
            {
                Debug.Log("Da vao v�y");
                PlayerHeal playerHeal = other.GetComponent<PlayerHeal>();
                switch (typeCollider)
                {
                    case TypeCollider.JumpFx:
                        {
                            IsAttackJump = false;
                            float damageJumpFx = CaculateDamage(bullTankSetting.percentAttackJump);
                            if (playerHeal != null)
                            {
                                if (!IsAttackJump)
                                {
                                    HandlerDamageTypeCollider(playerHeal, damageJumpFx, TypeCollider.Leg, other, "PlayerHit");
                                    IsAttackJump = true;
                                }
                            }
                        }
                        break;
                    case TypeCollider.AxeFX:
                        {
                            IsAttackAxe = false;
                            float damageAxeFx = CaculateDamage(bullTankSetting.percentAttackAxe);
                            if (playerHeal != null)
                            {
                                if (!IsAttackAxe)
                                {
                                    playerHeal.ReducePlayerHeal((int)damageAxeFx, TypeCollider.Leg);
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

    private void HandlerDamageTypeCollider(PlayerHeal playerHeal, float damage, TypeCollider typecollider, Collider collider, string nameSoundSE)
    {
        playerHeal.ReducePlayerHeal((int)damage, typecollider);
        PlayFxBlood(collider);
        if (AudioManager.HasInstance) AudioManager.Instance.PlaySE(nameSoundSE);
    }
    private void HandlerDamageTypeCollider(PlayerHeal playerHeal, float damage, TypeCollider typecollider, GameObject obj, string nameSoundSE)
    {
        playerHeal.ReducePlayerHeal((int)damage, typecollider);
        PlayFxBlood(GetComponent<Collider>());
        if (AudioManager.HasInstance) AudioManager.Instance.PlaySE(nameSoundSE);
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
    public void SetTypeCollider(TypeCollider typeCollider) => throwAxeHitBox.TypeCollider = typeCollider;
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
    private void PlayFxBlood(GameObject collider)
    {
        GameObject bloodPrefabs;
        if (EffectManager.HasInstance)
        {
            bloodPrefabs = EffectManager.Instance.GetPrefabs("BloodFx");
            GameObject obj = Instantiate(bloodPrefabs, collider.transform.position, Quaternion.identity);
            Destroy(obj, 2f);
        } 
            
    }    
}
