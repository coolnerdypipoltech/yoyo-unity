using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
public class RewardsInfoViewModel : ViewModel
{
    public TextMeshProUGUI titleText, descriptionText, validityText, conditionsText, costText, availableQuantityText;
    public RectTransform contentRebuild;
    public GameObject scrollSnapContainer, ImageGalleryContainer, ImageGalleryItemPrefab, tagContainer, togglePrefab;
    public ToggleGroup paginationToggleGroup;

    public TextMeshProUGUI noPointsText;
    public string link;

    private bool isFromRewards = false;
    public ScrollRect scrollRect;

    public Button redeemButton;

    /// <summary>
    /// Resets the view model when disabled to avoid data leakage between different places.

    void OnDisable()
    {
        scrollSnapContainer.SetActive(true);
        Destroy(scrollSnapContainer.GetComponent<SimpleScrollSnap>());
        titleText.text = "";
        descriptionText.text = "";
        validityText.text = "";
        conditionsText.text = "";
        costText.text = "";
        availableQuantityText.text = "";
        ImageGalleryContainer.SetActive(true);
        foreach (Transform child in ImageGalleryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        redeemButton.interactable = true;
        noPointsText.gameObject.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1;
        foreach (Transform child in paginationToggleGroup.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public string FormatDateRange(string start, string end)
    {
        if (System.DateTime.TryParse(start, out var startDate) &&
            System.DateTime.TryParse(end, out var endDate))
        {
            string[] months = { "Jan.", "Feb.", "Mar.", "Apr.", "May.", "Jun.", "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec." };
            string formattedStart = $"{startDate.Day} {months[startDate.Month - 1]} {startDate.Year}";
            string formattedEnd = $"{endDate.Day} {months[endDate.Month - 1]} {endDate.Year}";
            return $"{formattedStart} - {formattedEnd}";
        }
        return start + " - " + end;
    }

    public void OnClickRedeem()
    {

        if (isFromRewards)
        {
            ApiManager.instance.GenerateWhatsAppMessage("Hello YoYo " + "%0A" + "I’d like to redeem the following reward: " + titleText.text + ". " + "%0A" + "My ID is " + ApiManager.instance.GetUserId());
        }
        else
        {
            ApiManager.instance.GenerateWhatsAppMessage("Hello YoYo " + "%0A" + "I would like to find this partner: " + titleText.text + ". " + "%0A" + "My ID is " + ApiManager.instance.GetUserId());
        }

    }

    /// <summary>
    /// Initializes the view model with the provided place data.

    public void InitializeViewModel(ResultObject _reward, bool _isFromRewards = false)
    {
        titleText.text = _reward.name;
        descriptionText.text = _reward.description;
        validityText.text = FormatDateRange(_reward.starts_on, _reward.ends_on);
        conditionsText.text = _reward.conditions;
        costText.text = _reward.cost.ToString() + " YoYo Credits";
        availableQuantityText.text = _reward.stock.ToString();
        link = _reward.url;
        isFromRewards = _isFromRewards;

        SimpleScrollSnap scrollSnap = scrollSnapContainer.AddComponent<SimpleScrollSnap>();
        scrollSnap.AutomaticLayoutSpacing = 0f;
        if(_reward.gallery != null && _reward.gallery.Length > 1)
        {
            scrollSnap.Pagination = paginationToggleGroup;
            scrollSnap.ToggleNavigation = true;
        }
        if (_reward.gallery != null && _reward.gallery.Length > 0)
        {
            foreach (var media in _reward.gallery)
            {
                GameObject imageItem = Instantiate(ImageGalleryItemPrefab, ImageGalleryContainer.transform);
                if(_reward.gallery.Length > 1)
                {
                    GameObject toggleItem = Instantiate(togglePrefab, paginationToggleGroup.transform);
                    toggleItem.GetComponent<Toggle>().group = paginationToggleGroup;
                }
                if( media.type.ToLower() == "video")
                {
                    imageItem.GetComponent<ImageInterface>().setVideo(media.absolute_url);

                }
                else
                {
                    imageItem.GetComponent<ImageInterface>().takeOutMask();
                    ApiManager.instance.SetImageFromUrl(media.absolute_url, (Sprite response) =>
                    {
                        
                        imageItem.GetComponent<ImageInterface>().setImage(response);
                    });
                }
            }
        }
        else
        {
            if (_reward.media == null || _reward.media.Length == 0)
            {
                scrollSnapContainer.SetActive(false);
            }
            else
            {
                GameObject imageItem = Instantiate(ImageGalleryItemPrefab, ImageGalleryContainer.transform);
                imageItem.GetComponent<ImageInterface>().takeOutMask();
                ApiManager.instance.SetImageFromUrl(_reward.media[0].absolute_url, (Sprite response) =>
                {
                    
                    imageItem.GetComponent<ImageInterface>().setImage(response);
                });
            }


        }

        isRewardAvailable(_reward, isFromRewards);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tagContainer.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRebuild);
        StartCoroutine(WaitAFrame());
    }

    private void isRewardAvailable(ResultObject _reward, bool isFromRewards)
    {
        bool flag = true;
        if (ApiManager.instance.GetUsersPoints() < _reward.cost)
        {
            redeemButton.interactable = false;
            noPointsText.gameObject.SetActive(true);
            if(isFromRewards)
                noPointsText.text = "You don't have enough points to redeem this reward.";
            else
                noPointsText.text = "You don't have enough points to find this partner.";
            flag = false;
        }

        if (_reward.stock <= 0)
        {
            redeemButton.interactable = false;
            noPointsText.gameObject.SetActive(true);
            if(isFromRewards)
                noPointsText.text = "This reward is out of stock.";
            else
                noPointsText.text = "This partner is not available.";
            flag = false;
        }

        if (System.DateTime.TryParse(_reward.starts_on, out var startDate) &&
            System.DateTime.TryParse(_reward.ends_on, out var endDate))
        {
            var now = System.DateTime.Now;
            if (now < startDate || now > endDate)
            {
                redeemButton.interactable = false;
                noPointsText.gameObject.SetActive(true);
                if(isFromRewards)
                    noPointsText.text = "This reward is not available at this time.";
                else
                    noPointsText.text = "This partner is not available at this time.";
                flag = false;
            }
        }
        if (flag)
        {
            redeemButton.interactable = true;
            noPointsText.gameObject.SetActive(false);
        }

    }

    IEnumerator WaitAFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRebuild);
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tagContainer.GetComponent<RectTransform>());
    }

}
