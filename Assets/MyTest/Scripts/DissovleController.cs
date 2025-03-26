using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissovleController : MonoBehaviour
{

    public SkinnedMeshRenderer SkinnedMesh;
    public VisualEffect VFXGraph;
    public float dissolveRate;

    private Material skineshMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SkinnedMesh != null)
            skineshMaterial = SkinnedMesh.material;   
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(DissolveCo());

        //}
    }
    public IEnumerator DissolveCo()
    {
        if(VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }    

        if(skineshMaterial != null)
        {
            float t = 0f;
            while(t <1)
            {
                t += dissolveRate * Time.deltaTime;
                skineshMaterial.SetFloat("_DissvoleAmount", Mathf.Lerp(0f, 1f, t));
                yield return null; 
            }
        }
    }
}
