using UnityEngine;
using UnityEngine.UI;

public static class ScrollExtensions
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0,
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
