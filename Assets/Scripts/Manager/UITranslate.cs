using UnityEngine;

public static class UITranslate  
{
    // helper chuy?n world-pos ? anchoredPosition
    public static Vector2 WorldToAnchored(Vector3 worldPos , RectTransform canvas)
    {
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas, screenP, null, out Vector2 anchored);
        return anchored;
    }
}
