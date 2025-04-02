using System.Collections;
using UnityEngine;

public class RevealTextEffect : MonoBehaviour
{
    public Material textMaterial; // Gán Material c?a TextMeshPro
    public float duration = 2f;   // Th?i gian ch?y hi?u ?ng

    private void Start()
    {
        StartCoroutine(RevealText());
    }

    IEnumerator RevealText()
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float cutoffValue = elapsedTime / duration; // T?ng t? 0 ? 1
            textMaterial.SetFloat("_Cutoff", cutoffValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMaterial.SetFloat("_Cutoff", 1); // ??m b?o ch? xu?t hi?n hoàn toàn
    }
}
