using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using UnityEngine.Video;
public class PlacesInfoViewModel : ViewModel
{
    public TextMeshProUGUI titleText, descriptionText, musicLineUpText, locationText;

    public GameObject musicLineUpContainer, locationContainer, timeContainer, TagsContainer, tagParent, tagItemPrefab, FacebookButton, InstagramButton, WebsiteButton;
    public GameObject costRateContainer, paymentOptionsContainer, dressCodeContainer, socialMediaContainer, timeTextPrefab, timeIcon, timeTextContainer;
    public RectTransform contentRebuild;
    public ToggleGroup paginationToggleGroup;
    public GameObject ImageGalleryContainer, ImageGalleryItemPrefab, scrollSnapContainer, togglePrefab;

    public List<GameObject> costRate = new List<GameObject>();
    public List<GameObject> paymentOptions = new List<GameObject>();
    public List<GameObject> dressCodeOptions = new List<GameObject>();

    private bool isFromPlace = false;
    public ScrollRect scrollRect;
    private string eventLink = "";
    private string googleMapsLink = "";

    /// <summary>
    /// Resets the view model when disabled to avoid data leakage between different places.

    void OnDisable()
    {
        scrollSnapContainer.SetActive(true);
        Destroy(scrollSnapContainer.GetComponent<SimpleScrollSnap>());
        titleText.text = "";
        descriptionText.text = "";
        WebsiteButton.SetActive(true);
        FacebookButton.SetActive(true);
        InstagramButton.SetActive(true);
        WebsiteButton.GetComponent<Button>().onClick.RemoveAllListeners();
        FacebookButton.GetComponent<Button>().onClick.RemoveAllListeners();
        InstagramButton.GetComponent<Button>().onClick.RemoveAllListeners();
        musicLineUpContainer.SetActive(true);
        musicLineUpText.text = "";
        locationContainer.SetActive(true);
        locationText.text = "";
        timeContainer.SetActive(true);
        foreach (Transform child in TagsContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        paymentOptionsContainer.SetActive(true);
        foreach (var option in paymentOptions)
        {
            option.SetActive(false);
        }
        costRateContainer.SetActive(true);
        foreach (var item in costRate)
        {
            item.SetActive(false);
        }
        dressCodeContainer.SetActive(true);
        tagParent.SetActive(true);
        paymentOptionsContainer.SetActive(true);
        socialMediaContainer.SetActive(true);
        costRateContainer.SetActive(true);
        dressCodeOptions[0].SetActive(false);
        dressCodeOptions[1].SetActive(false);

        ImageGalleryContainer.SetActive(true);

        foreach (Transform child in timeTextContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        foreach (Transform child in ImageGalleryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        isFromPlace = false;
        eventLink = "";
        googleMapsLink = "";
        scrollRect.verticalNormalizedPosition = 1;
        foreach (Transform child in paginationToggleGroup.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void OnClickReserve()
    {
        if (isFromPlace)
        {
            
            string whatsappMessage = "Hello, I would like to make a reservation. In " + titleText.text + "." + " This is my Id: " + ApiManager.instance.GetUserId();
            ApiManager.instance.GenerateWhatsAppMessage(whatsappMessage);
        }
        else
        {
            if (eventLink != "")
            {
                Application.OpenURL(eventLink);
            }
             
        }
        
    }
    
    public void OnClickGetDirections()
    {
        if (googleMapsLink != "")
        {
            Application.OpenURL(googleMapsLink);
        }
        
    }


    /// <summary>
    /// Initializes the view model with the provided place data.

    public void InitializeViewModel(Place _place, bool _isFromPlace = false)
    {

        isFromPlace = _isFromPlace;
        titleText.text = _place.name;
        descriptionText.text = _place.description;
        googleMapsLink = _place.gmaps;
        if (_place.music_genre_list != null && _place.music_genre_list.Count > 0)
        {
            if (_place.music_genre_list[0] != "" && _place.music_genre_list.Count != 1)
            {
                foreach (var genre in _place.music_genre_list)
                {
                    float width = 0;
                    GameObject tag = Instantiate(tagItemPrefab, TagsContainer.transform);
                    tag.GetComponentInChildren<TextMeshProUGUI>().text = genre.Trim();
                    width = tag.GetComponentInChildren<TextMeshProUGUI>().preferredWidth + 20;
                    tag.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);
                    tag.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);
                    tag.GetComponentInChildren<ContentSizeFitter>().enabled = false;
                    tag.GetComponentInChildren<TextMeshProUGUI>().gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);

                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(TagsContainer.GetComponent<RectTransform>());
            }
            else
            {
                tagParent.SetActive(false);
            }
        }
        else
        {
            tagParent.SetActive(false);
        }

        SimpleScrollSnap scrollSnap = scrollSnapContainer.AddComponent<SimpleScrollSnap>();
        if(_place.gallery != null && _place.gallery.Count > 1)
        {
            scrollSnap.Pagination = paginationToggleGroup;
            scrollSnap.ToggleNavigation = true;
        }

        if (_place.gallery != null && _place.gallery.Count > 0)
        {
            foreach (var media in _place.gallery)
            {
                GameObject imageItem = Instantiate(ImageGalleryItemPrefab, ImageGalleryContainer.transform);
                if(_place.gallery.Count > 1)
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
                    ApiManager.instance.SetImageFromUrl(media.absolute_url, (Sprite response) =>
                    {
                        imageItem.GetComponent<ImageInterface>().setImage(response);
                    });
                }
            }
        }
        else
        {
            if (_place.media == null || _place.media.Count == 0)
            {
                scrollSnapContainer.SetActive(false);
            }
            else
            {
                GameObject imageItem = Instantiate(ImageGalleryItemPrefab, ImageGalleryContainer.transform);
                ApiManager.instance.SetImageFromUrl(_place.media[0].absolute_url, (Sprite response) =>
                {
                    imageItem.GetComponent<ImageInterface>().setImage(response);
                });
            }


        }

        if (_place.payment_options_list.Count == 0 || _place.payment_options_list[0] == "")
        {
            paymentOptionsContainer.SetActive(false);
        }
        else
        {
            for (int i = 0; i < _place.payment_options_list.Count; i++)
            {
                if (_place.payment_options_list[i].ToLower().Contains("cash"))
                {
                    paymentOptions[0].SetActive(true);
                }
                if (_place.payment_options_list[i].ToLower().Contains("card"))
                {
                    paymentOptions[1].SetActive(true);
                }

            }
        }

        if (_place.schedule_list.Count == 0 || _place.schedule_list[0] == "")
        {
            timeContainer.SetActive(false);
        }
        else
        {
            for (int i = 0; i < _place.schedule_list.Count; i++)
            {
                GameObject timeItem = Instantiate(timeTextPrefab, timeTextContainer.transform);
                timeItem.GetComponent<TextMeshProUGUI>().text = _place.schedule_list[i];
            }
            timeIcon.transform.position = new Vector2(timeIcon.transform.position.x, timeContainer.transform.position.y);
        }


        if (_place.dresscode == null || _place.dresscode == "")
        {
            dressCodeContainer.SetActive(false);
        }
        else
        {
            if (_place.dresscode.ToLower().Contains("formal"))
            {
                dressCodeOptions[0].SetActive(true);
            }
            if (_place.dresscode.ToLower().Contains("casual"))
            {
                dressCodeOptions[1].SetActive(true);
            }

        }


        if (_place.cost_rate == null || _place.cost_rate == "")
        {
            costRateContainer.SetActive(false);
        }


        eventLink = _place.url;
        if (_place.website_url == "")
        {

            WebsiteButton.SetActive(false);
        }
        else
        {
            WebsiteButton.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(_place.website_url));
        }

        if (_place.facebook_url == "")
        {
            FacebookButton.SetActive(false);
        }
        else
        {
            FacebookButton.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(_place.facebook_url));
        }

        if (_place.instagram_url == "")
        {
            InstagramButton.SetActive(false);
        }
        else
        {
            InstagramButton.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(_place.instagram_url));
        }

        if (WebsiteButton.activeSelf == false && FacebookButton.activeSelf == false && InstagramButton.activeSelf == false)
        {
            socialMediaContainer.SetActive(false);
        }


        if (_place.music_lineup == "")
        {
            musicLineUpContainer.SetActive(false);
        }
        else
        {
            musicLineUpText.text = _place.music_lineup;
        }
        if (_place.address == "")
        {
            locationContainer.SetActive(false);
        }
        else
        {
            locationText.text = _place.address;
        }
        if (_place.cost_rate != null && _place.cost_rate != "")
        {

            for (int i = 0; i < costRate.Count; i++)
            {
                if (i < _place.cost_rate.Length)
                {
                    costRate[i].SetActive(true);
                }
                else
                {
                    costRate[i].SetActive(false);
                }
            }
        }
        else
        {
            foreach (var item in costRate)
            {
                item.SetActive(false);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRebuild);
        StartCoroutine(WaitAFrame());
        
    }

    IEnumerator WaitAFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRebuild);
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TagsContainer.GetComponent<RectTransform>());
    }

}
