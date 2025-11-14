using UnityEngine;
using TMPro;
using System;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;
using System.Collections;
public class RewardsViewModel : ViewModel
{
    public TextMeshProUGUI pointsText;

    public GameObject ItemPrefab, rewardsContainer, noRewardsIcon, rewardsLoadingIcon , paddingRightPerks, paddingLeftPerks, paddingRightPartners, paddingLeftPartners; 

    public GameObject partnersContainer, noPartnersText, partnersLoadingIcon;
    public GameObject scrollSnapContainer, ImageGalleryContainer, ImageGalleryItemPrefab, parentContainer, pointsContainer, togglePrefab;
    public ToggleGroup paginationToggleGroup;

    private bool gettingMoreRewards, gettingMorePartners;
    public Root rewardsResponse;
    public Root partnersResponse;
    public advertisement ads;

    private bool cardValue;

    void Start()
    {
        GetRewards();
        GetPartners();
        GetAds();
    }

    void OnEnable()
    {
        User user = ApiManager.instance.GetUser();
        if (user != null)
        {
            pointsText.text = $"Available Points: {user.related.points}";
        }
        StartCoroutine(WaitAFrame());
    }

    public void OnClickOpenConfig()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.ConfigViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<ConfigViewModel>().enableWithRewards();

    }

    private void HideAds()
    {

        Destroy(scrollSnapContainer.GetComponent<SimpleScrollSnap>());
        StartCoroutine(WaitAFrame());
        foreach (Transform child in ImageGalleryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in paginationToggleGroup.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        scrollSnapContainer.SetActive(false);
    }

    IEnumerator WaitAFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentContainer.GetComponent<RectTransform>());
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(pointsContainer.GetComponent<RectTransform>());
    }

    public void ReloadAll()
    {
        OnClickReloadRewards();
        OnClickReloadPartners();
        HideAds();
        GetAds();
        ApiManager.instance.GetInfoFromToken((object[] response) => {
            User user = ApiManager.instance.GetUser();

            if (user != null)
            {
                pointsText.text = $"Available Points: {user.related.points}";
            }
            StartCoroutine(WaitAFrame());
            
        });
    }


    public void GetAds()
    {
        scrollSnapContainer.SetActive(true);

        ApiManager.instance.GetAdvertisements((object[] response) =>
        {
            SimpleScrollSnap scrollSnap = scrollSnapContainer.AddComponent<SimpleScrollSnap>();
            scrollSnap.AutomaticLayoutSpacing = 0f;
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                advertisement _ads = JsonUtility.FromJson<advertisement>(responseText);
                ads = _ads;
                if(ads.results.Length > 1)
                {
                    scrollSnap.Pagination = paginationToggleGroup;
                    scrollSnap.ToggleNavigation = true;
                }
                if (ads.results != null && ads.results.Length > 0)
                {
                    foreach (var media in ads.results)
                    {

                        GameObject imageItem = Instantiate(ImageGalleryItemPrefab, ImageGalleryContainer.transform);
                        if(ads.results.Length > 1)
                        {
                            GameObject toggleItem = Instantiate(togglePrefab, paginationToggleGroup.transform);
                            toggleItem.GetComponent<Toggle>().group = paginationToggleGroup;
                        }
                        if (media.main.type.ToLower() == "video")
                        {
                            imageItem.GetComponent<ImageInterface>().setVideo(media.main.absolute_url);
                            imageItem.GetComponent<ImageInterface>().setAdLink(media.url);
                        }
                        else
                        {
                            ApiManager.instance.SetImageFromUrl(media.main.absolute_url, (Sprite response) =>
                            {
                                imageItem.GetComponent<ImageInterface>().setImage(response);
                                imageItem.GetComponent<ImageInterface>().setAdLink(media.url);
                            });
                        }
                    }
                }
                else
                {
                    HideAds();
                }
            }
            else
            {
                HideAds();
            }

        });
    }

    public void OnClickReloadRewards()
    {
        foreach (Transform child in rewardsContainer.transform)
        {
            if (child.name == ItemPrefab.name + "(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        noRewardsIcon.SetActive(false);
        GetRewards();
    }

    public void OnClickReloadPartners()
    {
        foreach (Transform child in partnersContainer.transform)
        {
            if (child.name == ItemPrefab.name + "(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        noPartnersText.SetActive(false);
        GetPartners();
    }



    public void OnValueChangedRewardsSlider(Vector2 value)
    {
        if (value.x > 1.02f)
        {
            if (gettingMoreRewards) return;
            GetMoreRewards();
        }
    }

    public void OnValueChangedPartnersSlider(Vector2 value)
    {
        if (value.x > 1.02f)
        {
            if (gettingMorePartners) return;
            GetMorePartners();
        }
    }

    private void GetMoreRewards()
    {
        if (rewardsResponse.next != null && rewardsResponse.next != "")
        {
            rewardsLoadingIcon.SetActive(true);
            gettingMoreRewards = true;
            ApiManager.instance.GetMoreRewards(rewardsResponse.next, (object[] response) =>
            {

                long responseCode = (long)response[0];
                string responseText = response[1].ToString();
                if (responseCode == 200)
                {
                    Root moreRewards = JsonUtility.FromJson<Root>(responseText);
                    rewardsResponse.next = moreRewards.next;
                    rewardsResponse.previous = moreRewards.previous;
                    rewardsResponse.results.AddRange(moreRewards.results);
                    GetRewardsCallback(moreRewards.results.ToArray());
                }
                else
                {
                    rewardsLoadingIcon.SetActive(false);
                    gettingMoreRewards = false;
                }
            });
        }
    }

    private void GetRewards()
    {

        ApiManager.instance.GetRewards(10, 0, (object[] response) =>
        {

            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                rewardsResponse = JsonUtility.FromJson<Root>(responseText);

                GetRewardsCallback(rewardsResponse.results.ToArray());
            }


        });
    }
    private void GetRewardsCallback(ResultObject[] results, bool isEvent = false)
    {
        if (rewardsResponse.total == 0)
        {
            noRewardsIcon.SetActive(true);
            return;
        }

        foreach (var item in results)
        {
            GameObject placeItem = Instantiate(ItemPrefab, rewardsContainer.transform);
            placeItem.GetComponent<RewardInterface>().SetPlace(item, true);
        }
        paddingLeftPerks.transform.SetAsFirstSibling();
        rewardsLoadingIcon.transform.SetAsLastSibling();
        paddingRightPerks.transform.SetAsLastSibling();
        
        gettingMoreRewards = false;
        rewardsLoadingIcon.SetActive(false);
    }

    private void GetPartners()
    {
        ApiManager.instance.GetPartners(10, 0, (object[] response) =>
        {

            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                partnersResponse = JsonUtility.FromJson<Root>(responseText);

                GetPartnersCallback(partnersResponse.results.ToArray());
            }


        });
    }

    private void GetPartnersCallback(ResultObject[] results)
    {
        if (partnersResponse.total == 0)
        {
            noPartnersText.SetActive(true);
            return;
        }

        foreach (var item in results)
        {
            GameObject Item = Instantiate(ItemPrefab, partnersContainer.transform);
            Item.GetComponent<RewardInterface>().SetPlace(item, false);
        }
        paddingLeftPartners.transform.SetAsFirstSibling();
        partnersLoadingIcon.transform.SetAsLastSibling();
        paddingRightPartners.transform.SetAsLastSibling();
        
        gettingMorePartners = false;
        partnersLoadingIcon.SetActive(false);
    }

    private void GetMorePartners()
    {
        if (partnersResponse.next != null && partnersResponse.next != "")
        {
            partnersLoadingIcon.SetActive(true);
            gettingMorePartners = true;
            ApiManager.instance.GetMorePartners(partnersResponse.next, (object[] response) =>
            {

                long responseCode = (long)response[0];
                string responseText = response[1].ToString();
                if (responseCode == 200)
                {
                    Root morePartners = JsonUtility.FromJson<Root>(responseText);
                    partnersResponse.next = morePartners.next;
                    partnersResponse.previous = morePartners.previous;
                    partnersResponse.results.AddRange(morePartners.results);
                    GetPartnersCallback(partnersResponse.results.ToArray());
                }
                else
                {
                    partnersLoadingIcon.SetActive(false);
                    gettingMorePartners = false;
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
