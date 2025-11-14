using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Detects a downward vertical touch gesture using the Unity Input System (EnhancedTouch)
/// and temporarily disables the associated ScrollRect while the gesture is active.
/// </summary>
public class testScroll : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    [Tooltip("Optional: Camera used for UI hit testing (leave null to use Camera.main).")]
    public Camera uiCamera;

    [Header("Gesture Settings")]
    [Tooltip("Minimum vertical pixels before it's considered a vertical gesture.")]
    public float minVerticalGesture = 20f;
    [Tooltip("If true, only disables on downward finger movement (content would scroll down).")]
    public bool onlyWhenGoingDown = true;
    [Tooltip("Zero out velocity when disabling to stop inertial motion.")]
    public bool zeroOutVelocity = true;
    [Tooltip("Max fingers to consider; additional fingers are ignored.")]
    public int maxFingerCount = 1;

    private Vector2 startPos;
    private bool tracking;
    private bool gestureActive;
    private Finger trackedFinger; // Finger being tracked

    void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();
        if (uiCamera == null)
            uiCamera = Camera.main;
    }

    void OnEnable()
    {
        // Enable Enhanced Touch support
        if (!EnhancedTouch.EnhancedTouchSupport.enabled)
            EnhancedTouch.EnhancedTouchSupport.Enable();

        EnhancedTouch.TouchSimulation.Enable(); // (Optional) lets mouse act as touch in editor

        EnhancedTouch.Touch.onFingerDown += HandleFingerDown;
        EnhancedTouch.Touch.onFingerUp += HandleFingerUp;
        EnhancedTouch.Touch.onFingerMove += HandleFingerMove;
    }

    void OnDisable()
    {
        // Unsubscribe & cleanup
    EnhancedTouch.Touch.onFingerDown -= HandleFingerDown;
    EnhancedTouch.Touch.onFingerUp -= HandleFingerUp;
    EnhancedTouch.Touch.onFingerMove -= HandleFingerMove;

        FinishGesture();
    }

    private void HandleFingerDown(EnhancedTouch.Finger finger)
    {
        Debug.Log("Finger Down Detected");
        if (trackedFinger != null) return;
        Debug.Log("1"); // Already tracking a finger
        if (maxFingerCount <= 0) return;
        Debug.Log("2");
        if (EnhancedTouch.Touch.activeFingers.Count > maxFingerCount) return;
        Debug.Log("3");
        // Must start within the ScrollRect viewport area
        if (scrollRect == null || scrollRect.viewport == null) return;
        Debug.Log("4");
        Debug.Log("5");
        trackedFinger = finger;
        startPos = finger.screenPosition;
        tracking = true;
        gestureActive = false;
    }

    private void HandleFingerMove(EnhancedTouch.Finger finger)
    {
        if (!tracking || finger != trackedFinger || scrollRect == null) return;

        Vector2 currentPos = finger.screenPosition;
        Vector2 delta = currentPos - startPos;

        // Detect vertical intent
        if (!gestureActive &&
            Mathf.Abs(delta.y) >= minVerticalGesture &&
            Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            // In screen coordinates, downward movement reduces y (delta.y < 0)
            if (!onlyWhenGoingDown || delta.y < 0f)
            {
                Debug.Log("Gesture Detected");
                if (zeroOutVelocity) scrollRect.velocity = Vector2.zero;
                Debug.Log("Gesture Detected 2");
                scrollRect.enabled = false;
                gestureActive = true;
            }
        }
    }

    private void HandleFingerUp(EnhancedTouch.Finger finger)
    {
        if (finger == trackedFinger)
        {
            FinishGesture();
        }
    }

    private void FinishGesture()
    {
        if (scrollRect != null)
            scrollRect.enabled = true;
        tracking = false;
        gestureActive = false;
        trackedFinger = null;
    }
}
