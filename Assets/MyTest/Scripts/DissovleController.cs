using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissovleController : MonoBehaviour
{

    public SkinnedMeshRenderer SkinnedMesh;
    public VisualEffect VFXGraph;
    public float dissolveRate;

    public Material skineshMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if (SkinnedMesh != null)
        {
            skineshMaterial = SkinnedMesh.sharedMaterial;
            skineshMaterial.SetFloat("_DissvoleAmount", 0f);
        }    
            
    }
    public IEnumerator DissolveCo()
    {
        yield return new WaitForSeconds(1f);
        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }

        if (skineshMaterial != null)
        {
            float t = 0f;
            while (t < 1)
            {
                t += dissolveRate * Time.deltaTime;
                skineshMaterial.SetFloat("_DissvoleAmount", Mathf.Lerp(0f, 1f, t));
                yield return null;
            }
        }
    }
}
