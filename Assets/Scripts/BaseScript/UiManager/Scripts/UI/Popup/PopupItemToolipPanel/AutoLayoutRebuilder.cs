using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class AutoLayoutRebuilder : MonoBehaviour
{
    [Tooltip("Th?i gian delay sau khi thay ??i ?? rebuild layout (?? phòng frame ch?a tính xong).")]
    public float delayBeforeRebuild = 0.01f;

    private RectTransform rectTransform;
    private bool needRebuild = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartCoroutine(DelayedRebuild());
    }

    void OnTransformChildrenChanged()
    {
        TriggerRebuild();
    }

    void OnRectTransformDimensionsChange()
    {
        TriggerRebuild();
    }

    public void TriggerRebuild()
    {
        if (!gameObject.activeInHierarchy) return;

        if (!needRebuild)
        {
            needRebuild = true;
            StartCoroutine(DelayedRebuild());
        }
    }

    IEnumerator DelayedRebuild()
    {
        yield return new WaitForSeconds(delayBeforeRebuild);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        needRebuild = false;
    }
}
