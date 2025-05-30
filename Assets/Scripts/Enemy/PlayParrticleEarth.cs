using System.Collections;
using UnityEngine;

public class PlayParrticleEarth : MonoBehaviour
{
    [SerializeField] private ParticleSystem earthSkill;
    [SerializeField] private GameObject earthSkillContainer;
    [SerializeField] private GameObject earthSkillGameObj;


    public void PlayEarth()
    {
        if (earthSkill != null)
        {
            earthSkillGameObj.transform.position = earthSkillContainer.transform.position;  
            earthSkill.Play();
        }
       

    }
   
}
