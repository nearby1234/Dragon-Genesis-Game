using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TxtProgress;
    [SerializeField] private Slider m_ProgressSlider;
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Project RPG");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            m_ProgressSlider.value = asyncOperation.progress;
            m_TxtProgress.SetText($"Loading : {asyncOperation.progress * 100}%");

            if(asyncOperation.progress >=0.9f)
            {
                m_ProgressSlider.value = 1f;
                m_TxtProgress.SetText($"Loading : {m_ProgressSlider.value * 100}%");

                yield return new WaitForSeconds(1f);
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
       
    }


}
