using UnityEngine;
using DG.Tweening;

public class ConfigViewModel : ViewModel
{
    public GameObject configContainer, placeVM, rewardsVM;
    public RectTransform finalPosition, firstPosition;
    public MenuButtonInterface menuButtonInterface, menuButtonInterface2;

    private string type;

    void OnEnable()
    {
        if (type == "places")
        {
            enableWithPlaces();
        }
        else if (type == "rewards")
        {
            enableWithRewards();
        }
    }

    void Start()
    {

    }

    public void enableWithRewards()
    {
        rewardsVM.SetActive(true);
        configContainer.transform.DOMoveY(finalPosition.transform.position.y, 0.25f);
        type = "rewards";
    }

    public void enableWithPlaces()
    {
        placeVM.SetActive(true);
        configContainer.transform.DOMoveY(finalPosition.transform.position.y, 0.25f);
        type = "places";
    }

    public void OnClickHide()
    {
        if (menuButtonInterface.gameObject.activeInHierarchy)
        {
            menuButtonInterface.closeAnimation();
        }
        else
        {
            if (menuButtonInterface2.gameObject.activeInHierarchy)
            {
                menuButtonInterface2.closeAnimation();
            }
        }

        configContainer.transform.DOMoveY(firstPosition.transform.position.y, 0.25f).OnComplete(() =>
        {
            if (NewScreenManager.instance.GetCurrentView().viewID != ViewID.ConfigViewModel)
            {
                return;
            }
            placeVM.SetActive(false);
            rewardsVM.SetActive(false);

            NewScreenManager.instance.BackToPreviousView();
            type = "";

        });
    }
    
    private void ForceCloseAnimation()
    {
        menuButtonInterface.HardReset();
        menuButtonInterface2.HardReset();
    }

    public void OnClickOpenProfile()
    {
        ForceCloseAnimation();
        NewScreenManager.instance.BackToPreviousView();
        NewScreenManager.instance.ChangeToMainView(ViewID.ProfileViewModel, true);
    }
    public void OnClickLogOut()
    {
        ForceCloseAnimation();
        ApiManager.instance.ClearToken();
        NewScreenManager.instance.ChangeToMainView(ViewID.WelcomeViewModel, false);
    }
    public void OnClickOpenFAQ()
    {
        ForceCloseAnimation();
        NewScreenManager.instance.BackToPreviousView();
        NewScreenManager.instance.ChangeToMainView(ViewID.FAQViewModel, true);
    }

    public void OnClickOpenTerms()
    {
        Application.OpenURL("https://github.com/");
    }
}
