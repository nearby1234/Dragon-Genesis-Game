using UnityEngine;
using UnityEngine.UI;

public class SliderToShader : MonoBehaviour
{
    public Slider slider;           // Gán Slider UI
    public Material material;       // Gán Material có dùng Shader Graph

    void Update()
    {
        // G?i giá tr? slider.value (0 -> 1) vào Shader
        material.SetFloat("_SliderValue", slider.value);
    }
}
