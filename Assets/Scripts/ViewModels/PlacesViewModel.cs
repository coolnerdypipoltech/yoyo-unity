using UnityEngine;
using TMPro;
using System;
public class PlacesViewModel : ViewModel
{
    public GameObject cardPopUpContainer;
    public GameObject placeItemPrefab, placesContainer, noPlacesText, placesLoadingIcon, paddingRightPlaces, paddingLeftPlaces, paddingRightEvents, paddingLeftEvents;

    public GameObject  eventsContainer, noEventsText, eventsLoadingIcon;

    private bool gettingMorePlaces, gettingMoreEvents;
    public PlacesResponse placesResponse;
    public PlacesResponse eventsResponse;

    public CardInterface cardInterface1, cardInterface2;

    private bool cardValue;

    void Start()
    {
        GetPlaces();
        GetEvents();
    }

    public void OnDoubleTap()
    {
        cardPopUpContainer.SetActive(true);
    }

    public void SetCardValue(bool value)
    {
        cardValue = value;
    }

    public bool GetCardValue()
    {
        return cardValue;
    }

    public void OnValueChangedPlacesSlider(Vector2 value)
    {
        if (value.x > 1.02f)
        {
            if (gettingMorePlaces) return;
            GetMorePlaces();
        }
    }

    public void OnValueChangedEventsSlider(Vector2 value)
    {
        if (value.x > 1.02f)
        {
            if(gettingMoreEvents) return;
            GetMoreEvents();
        }
    }

    public void ReloadAll()
    {
        ReloadPlaces();
        ReloadEvents();
        ApiManager.instance.GetInfoFromToken((object[] response) => {
            cardInterface1.UpdateUsersPoints();
            cardInterface2.UpdateUsersPoints();
            
        });

    }

    public void ReloadPlaces()
    {
        foreach (Transform child in placesContainer.transform)
        {
            if (child.name == placeItemPrefab.name + "(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }

        }
        
        noPlacesText.SetActive(false);
        placesLoadingIcon.SetActive(true);
        GetPlaces();
    }

    public void ReloadEvents()
    {
        foreach (Transform child in eventsContainer.transform)
        {
            if (child.name == placeItemPrefab.name + "(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        noEventsText.SetActive(false);
        eventsLoadingIcon.SetActive(true);
        GetEvents();
    }

    private void GetMorePlaces()
    {
        if (placesResponse.next != null && placesResponse.next != "")
        {
            placesLoadingIcon.SetActive(true);
            gettingMorePlaces = true;
            ApiManager.instance.GetMorePlaces(placesResponse.next, (object[] response) =>
            {

                long responseCode = (long)response[0];
                string responseText = response[1].ToString();
                if (responseCode == 200)
                {
                    PlacesResponse morePlaces = JsonUtility.FromJson<PlacesResponse>(responseText);
                    placesResponse.next = morePlaces.next;
                    placesResponse.prev = morePlaces.prev;
                    placesResponse.results.AddRange(morePlaces.results);
                    GetPlacesCallback(morePlaces.results.ToArray());
                    placesLoadingIcon.SetActive(false);
                }
                else
                {
                    placesLoadingIcon.SetActive(false);
                    gettingMorePlaces = false;
                }
            });
        }
    }

    private void GetPlaces()
    {
        ApiManager.instance.GetPlaces(10, 0, (object[] response) =>
        {

            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                placesResponse = JsonUtility.FromJson<PlacesResponse>(responseText);

                GetPlacesCallback(placesResponse.results.ToArray());
            }
            
        });
    }
    private void GetPlacesCallback(Place[] results, bool isEvent = false)
    {
        if (placesResponse.total == 0)
        {
            noPlacesText.SetActive(true);
            placesLoadingIcon.SetActive(false);
            return;
        }

        foreach (var item in results)
        {
            GameObject placeItem = Instantiate(placeItemPrefab, placesContainer.transform);
            placeItem.GetComponent<PlaceInterface>().SetPlace(item, true);
        }
        paddingLeftPlaces.transform.SetAsFirstSibling();
        placesLoadingIcon.transform.SetAsLastSibling();
        paddingRightPlaces.transform.SetAsLastSibling();
        
        gettingMorePlaces = false;
        placesLoadingIcon.SetActive(false);
    }

    private void GetEvents()
    {
        ApiManager.instance.GetEvents(10, 0, (object[] response) =>
        {

            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                eventsResponse = JsonUtility.FromJson<PlacesResponse>(responseText);

                GetEventsCallback(eventsResponse.results.ToArray());
            }

            
        });
    }

    private void GetEventsCallback(Place[] results)
    {
        if (eventsResponse.total == 0)
        {
            noEventsText.SetActive(true);
            eventsLoadingIcon.SetActive(false);
            return;
        }

        foreach (var item in results)
        {
            GameObject Item = Instantiate(placeItemPrefab, eventsContainer.transform);
            Item.GetComponent<PlaceInterface>().SetPlace(item, false);
        }
        paddingLeftEvents.transform.SetAsFirstSibling();
        eventsLoadingIcon.transform.SetAsLastSibling();
        paddingRightEvents.transform.SetAsLastSibling();
        
        gettingMoreEvents = false;
        eventsLoadingIcon.SetActive(false);
    }

    private void GetMoreEvents()
    {
        if (eventsResponse.next != null && eventsResponse.next != "")
        {
            eventsLoadingIcon.SetActive(true);
            gettingMoreEvents = true;
            ApiManager.instance.GetMorePlaces(eventsResponse.next, (object[] response) =>
            {

                long responseCode = (long)response[0];
                string responseText = response[1].ToString();
                if (responseCode == 200)
                {
                    PlacesResponse moreEvents = JsonUtility.FromJson<PlacesResponse>(responseText);
                    eventsResponse.next = moreEvents.next;
                    eventsResponse.prev = moreEvents.prev;
                    eventsResponse.results.AddRange(moreEvents.results);
                    GetEventsCallback(moreEvents.results.ToArray());
                }
                else
                {
                    eventsLoadingIcon.SetActive(false);
                    gettingMoreEvents = false;
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickOpenConfig()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.ConfigViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<ConfigViewModel>().enableWithPlaces();

    }
}
