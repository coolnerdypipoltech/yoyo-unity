using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;
using Unity.VisualScripting;
public class CardInterface : MonoBehaviour
{
    public GameObject cardFront, cardBack, contentValue, restPosition, cardPopUp;
    public TextMeshProUGUI pointsText, nameText, totalPointsText, idText;

    public RectTransform firstPosition, contentValueRestPosition;

    private float lastActivationTime = -1f;
    private float activationThreshold = 0.4f;

    private float animationDuration = 0.3f;

    private bool FirstAnimationDone, isAnimating;

    public bool isFullScreen, isDragging;


    void OnEnable()
    {
        if(NewScreenManager.instance.GetCurrentView().viewID != ViewID.PlacesViewModel)
        {
            return;
        }

        UpdateUsersPoints();
        
        
        if (isFullScreen)
        {
            gameObject.transform.DOMove(restPosition.transform.position, 0.5f, false).SetEase(Ease.InQuad);
        }

        if (NewScreenManager.instance.GetCurrentView().GetComponent<PlacesViewModel>().GetCardValue())
        {
            FirstAnimationDone = true;
            cardFront.SetActive(false);
            cardBack.SetActive(true);
            cardBack.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            FirstAnimationDone = false;
            cardFront.SetActive(true);
            cardBack.SetActive(false);
            cardFront.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void UpdateUsersPoints()
    {
        User user = ApiManager.instance.GetUser();
        if (user != null)
        {
            nameText.text = user.name;
            idText.text = FormatId(user.id);
            pointsText.text = $"{user.related.points} POINTS";
            totalPointsText.text = $"{user.related.total_points} POINTS";
        }
    }

    public void SetIsDragging(bool value)
    {
        isDragging = value;
    }

    public string FormatId(int id)
    {
        return id.ToString().PadLeft(6, '0');
    }

    void OnDisable()
    {
        if (isFullScreen)
        {
            gameObject.transform.DOMove(firstPosition.position, 0.5f).SetEase(Ease.InQuad);
        }

    }

    public void OnClickClose(GameObject _gameObject)
    {
        gameObject.transform.DOMove(firstPosition.position, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            _gameObject.SetActive(false);
        });
        

    }

    public void OnClickAddPoints()
    {
        Debug.Log("Add Points Clicked" + FirstAnimationDone);
        if (!FirstAnimationDone)
        {
            return;
        }
        
        ApiManager.instance.GenerateWhatsAppMessage("Hello, I would like to add points to my account." + " This is my Id: " + ApiManager.instance.GetUserId());
    }

    public void onDoubleTap()
    {
        float currentTime = Time.time;

        if (currentTime - lastActivationTime <= activationThreshold)
        {
            cardPopUp.SetActive(true);
        }

        lastActivationTime = currentTime;
    }

    public void OnValueChange(Vector2 value)
    {
        if(!isDragging)
        {
            return;
        }
        


        if (Math.Abs(value.y) > Math.Abs(value.x))
        {
            return;
        }

        if (isFullScreen)
        {
            if (contentValue.transform.position.x == firstPosition.position.x)
            {
                return;
            }
        }
        if (isAnimating)
        {
            return;
        }


        if (!FirstAnimationDone)
        {
            Debug.Log("1");
            PlayAnimationFirst();
            NewScreenManager.instance.GetCurrentView().GetComponent<PlacesViewModel>().SetCardValue(true);
        }
        else
        {
            Debug.Log("2");
            PlayAnimationSecond();
            NewScreenManager.instance.GetCurrentView().GetComponent<PlacesViewModel>().SetCardValue(false);
        }
        
    }

    private IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(0.5f);
        contentValue.transform.position = contentValueRestPosition.position;
            isAnimating = false;
    }

    public void PlayAnimationFirst()
    {
        isAnimating = true;
        cardFront.transform.DORotate(new Vector3(0, 90, 0), animationDuration).OnComplete(() => PlayAnimationFirstCallback());
    }

    private void PlayAnimationFirstCallback()
    {
        cardFront.SetActive(false);
        cardBack.transform.eulerAngles = new Vector3(0, 90, 0);
        cardBack.SetActive(true);
        cardBack.transform.DORotate(new Vector3(0, 180, 0), animationDuration).OnComplete(() =>
        {
            StartCoroutine(WaitAndReset());
            FirstAnimationDone = true;
        });
    }

    public void PlayAnimationSecond()
    {
        isAnimating = true;
        cardBack.transform.DORotate(new Vector3(0, 90, 0), animationDuration).OnComplete(()=>PlayAnimationSecondCallback());
    }

    private void PlayAnimationSecondCallback()
    {
        cardFront.SetActive(false);
        cardFront.SetActive(true);
        cardFront.transform.eulerAngles = new Vector3(0, 90, 0);
        cardFront.transform.DORotate(new Vector3(0, 0, 0), animationDuration).OnComplete(() =>
        {
            StartCoroutine(WaitAndReset());
            FirstAnimationDone = false;
        });
    }
}
