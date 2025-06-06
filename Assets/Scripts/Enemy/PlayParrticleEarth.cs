using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class PlayParrticleEarth : MonoBehaviour
{
    [SerializeField] private ParticleSystem earthSkill;
    [SerializeField] private ParticleSystem forceJump;
    [SerializeField] private ParticleSystem thunderWeapon;
    [SerializeField] private ParticleSystem trackZone;
    [SerializeField] private ParticleSystem ThunderArmor;
    [SerializeField] private ParticleSystem AxeFire;
    [SerializeField] private float posZTrackZone;
    [SerializeField] private GameObject earthSkillContainer;
    [SerializeField] private GameObject earthSkillGameObj;
    [SerializeField] private BullTankHeal bullTankHeal;
    [SerializeField] private BehaviorGraphAgent graphAgent;

    private void Awake()
    {
        bullTankHeal = GetComponent<BullTankHeal>();

    }
    private void Start()
    {
        if (bullTankHeal != null)
        {
            bullTankHeal.OnActionAgent += ReceiverActionEvent;
        }
    }
    private void OnDestroy()
    {
        bullTankHeal.OnActionAgent -= ReceiverActionEvent;
    }

    // Animation Event  => Animation Clip : Jump
    public void PlayEarth()
    {
        if (earthSkill != null)
        {
            earthSkillGameObj.transform.position = earthSkillContainer.transform.position;
            earthSkill.Play();
            if (ListenerManager.HasInstance)
            {
                string nameSound = "SFX-Spell-03-Earth_wav";
                ListenerManager.Instance.BroadCast(ListenType.PLAYSOUNDSE_BOSSBULLTANK, nameSound);
            }
        }
    }
    // Animation Event  => Animation Clip : Jump
    public void PlayTrackZone()
    {
        trackZone.transform.SetParent(null);
        trackZone.transform.position = transform.position + transform.forward * posZTrackZone;
        trackZone.gameObject.SetActive(true);
        trackZone.Play();
        StartCoroutine(SetParent(trackZone));
    }
    // Animation Event  => Animation Clip : Angry
    public void PlayThunderBolt()
    {
        if (VariablePhaseState())
        {
            thunderWeapon.gameObject.SetActive(true);
            thunderWeapon.Play();
        }
    }
    public void PlayThunderArmor()
    {
        if (VariablePhaseState())
        {
            ThunderArmor.gameObject.SetActive(true);
            ThunderArmor.Play();
        }

    }
    public void HideThunderArmor()
    {
        ThunderArmor.gameObject.SetActive(false);
        ThunderArmor.Stop();
    }
    // Animation Event  => Animation Clip : Jump
    public void PLayForceJump()
    {
        forceJump.transform.SetParent(null);
        forceJump.gameObject.SetActive(true);
        forceJump.Play();
        StartCoroutine(SetParent(forceJump));
    }
    public void HideAxeFire()
    {
        AxeFire.Stop();
    }

    IEnumerator SetParent(ParticleSystem particle)
    {
        yield return new WaitForSeconds(2f);
        particle.transform.SetParent(earthSkillContainer.transform);
        particle.transform.localPosition = Vector3.zero;
    }

    public void ShakeCamera()
    {
        if(CameraManager.HasInstance)
        {
            CameraManager.Instance.ShakeCamera();
        }
    }

    private void ReceiverActionEvent(BehaviorGraphAgent agent)
    {
        if (agent != null)
        {
            graphAgent = agent;
        }
    }
    private bool VariablePhaseState()
    {
        graphAgent.GetVariable<PhaseState>("PhaseStateBoss", out BlackboardVariable<PhaseState> variable);
        if (variable != null)
            if (variable.Value == PhaseState.Third)
            {
                return true;
            }
        return false;
    }

    

}
