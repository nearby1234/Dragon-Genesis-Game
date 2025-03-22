using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField]private  ParticleSystem shockWave;
   public void PlayShockWave()
    {
      if(shockWave != null)
        {
            shockWave.gameObject.SetActive(true);
            if (!shockWave.isPlaying)
            {
                shockWave.Play();
            }
        }    
    }  
    
    public void StopShockWave()
    {
        if (shockWave != null)
        {
            shockWave.gameObject.SetActive(false);
            if (shockWave.isPlaying)
            {
                shockWave.Stop();
            }    
        }
    }    
}
