using UnityEngine;
using DG.Tweening;
public class MenuButtonInterface : MonoBehaviour
{
    public GameObject buttonIcon;
    public PlacesViewModel placesViewModel;

    public RewardsViewModel rewardsViewModel;

    public bool handlingAnimation = false;

    private bool handlingCloseAni = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void handleAnimation()
    {
        if (handlingAnimation)
        {
            return;
        }
        handlingAnimation = true;
        DOTween.KillAll();
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gameObject.transform.DOScale(0.8f, 0.3f).onComplete += () =>
        {
            gameObject.transform.DOScale(1f, 0.3f).onComplete += () =>
            {
                handlingAnimation = false;
            };
        };
        buttonIcon.transform.DORotate(new Vector3(0, 0, 45), 0.3f);
        if (placesViewModel != null)
        {
            placesViewModel.OnClickOpenConfig();
        }
        else
        {
            rewardsViewModel.OnClickOpenConfig();
        }

    }
    
    public void HardReset()
    {
        handlingAnimation = false;
        handlingCloseAni = false;
        buttonIcon.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    
    public void closeAnimation()
    {
        if (handlingCloseAni)
        {
            return;
        }
        handlingCloseAni = true;
        DOTween.KillAll();
        gameObject.transform.localScale = Vector3.one;
        handlingAnimation = true;
        buttonIcon.transform.DORotate(new Vector3(0, 0, 0), 0.3f).onComplete += () =>
        {
            handlingAnimation = false;
            handlingCloseAni = false;
        };
    }
}
