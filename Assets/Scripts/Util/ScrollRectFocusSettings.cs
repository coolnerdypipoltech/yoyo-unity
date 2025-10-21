using UnityEngine;
using UnityEngine.UI;

public class ScrollRectFocusSettings : MonoBehaviour
{
    [SerializeField] private RectTransform m_scrollTopReference;
    [SerializeField]  private RectTransform m_scrollBottomReference;
    [SerializeField]  private RectTransform m_InputTextFocusHeightAdjuster, mInputTextFocusHeightAdjusterSmaller;

    private ScrollRect m_scrollRect;
    public ScrollRect scrollRect => m_scrollRect;

    private void Awake()
    {
        m_scrollRect = GetComponent<ScrollRect>();
    }

    public void OnSelectedInputText(RectTransform _child)
    {
        float childLength = Vector2.Distance(_child.position, m_scrollTopReference.position);

        Vector3 middleOfTheScreenWorldPosition = new Vector2(Screen.width / 2, Screen.height / 2);

        float middlePointLength = Vector2.Distance(middleOfTheScreenWorldPosition, m_scrollTopReference.position);

        if (childLength > middlePointLength)
        {

            if(childLength - middlePointLength < 600)
            {
                m_InputTextFocusHeightAdjuster.gameObject.SetActive(false);
                mInputTextFocusHeightAdjusterSmaller.gameObject.SetActive(true);
            }
            else
            {
                m_InputTextFocusHeightAdjuster.gameObject.SetActive(true);
                mInputTextFocusHeightAdjusterSmaller.gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_scrollRect.content as RectTransform);

            m_scrollRect.verticalNormalizedPosition = 0.0f;
        }

        m_scrollRect.enabled = false;
    }

    public void OnDeselect()
    {
        m_InputTextFocusHeightAdjuster.gameObject.SetActive(false);
        mInputTextFocusHeightAdjusterSmaller.gameObject.SetActive(false);
        m_scrollRect.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
