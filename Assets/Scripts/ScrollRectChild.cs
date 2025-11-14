using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
public class ScrollRectChild : ScrollRect
{
    public String test2;

    public ScrollRect ParentScrollRect;

    public String test;

    public override void OnScroll(PointerEventData data)
    {
        // Determine if the child can still scroll in the given direction
        // For vertical scrolling:
        bool canChildScrollUp = verticalNormalizedPosition < 1f;
        bool canChildScrollDown = verticalNormalizedPosition > 0f;

        if (data.scrollDelta.y > 0 && !canChildScrollUp) // Scrolling up, child reached top
        {
            if (ParentScrollRect != null)
            {
                ParentScrollRect.OnScroll(data); // Pass scroll to parent
            }
        }
        else if (data.scrollDelta.y < 0 && !canChildScrollDown) // Scrolling down, child reached bottom
        {
            if (ParentScrollRect != null)
            {
                ParentScrollRect.OnScroll(data); // Pass scroll to parent
            }
        }
        else // Child can still scroll
        {
            base.OnScroll(data);
        }
    }

    // Implement similar logic for OnDrag, OnInitializePotentialDrag, OnBeginDrag, OnEndDrag
    // to handle drag events passing to the parent.
}