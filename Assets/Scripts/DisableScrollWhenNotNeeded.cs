using UnityEngine;
using UnityEngine.UI;

public class DisableScrollWhenNotNeeded : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform viewport;

    void Update()
    {
        // If content is smaller than or equal to viewport, disable scrolling
        bool canScroll = content.rect.height > viewport.rect.height;
        scrollRect.vertical = canScroll;
    }
}